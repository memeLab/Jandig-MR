using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Controlador simples para criar/remover/restaurar uma `OVRSpatialAnchor` neste GameObject
/// usando OVRSpatialAnchor.SaveAnchorAsync / LoadUnboundAnchorsAsync + LocalizeAsync.
/// - A (Button.One): cria a anchor neste objeto (se não existir) e salva o UUID em memória;
///   se já houver UUID em memória, tenta carregar/localizar essa anchor e ligá‑la a este objeto.
/// - B (Button.Two): remove a anchor da cena (mantém UUID em memória para restauração).
/// </summary>
public class SpatialAnchorController : MonoBehaviour
{
    // Componente nativo de anchor atualmente ligado a este GameObject (se presente)
    private OVRSpatialAnchor _spatialAnchorComponent;

    // UUID salvo em memória (não persistente entre sessões) — usado para restaurar a anchor
    private string _anchorUuid;

    // Buffer para LoadUnboundAnchorsAsync
    private readonly List<OVRSpatialAnchor.UnboundAnchor> _unboundAnchors = new();

    // Callback usado ao terminar a localização (segue padrão do sample oficial)
    private Action<bool, OVRSpatialAnchor.UnboundAnchor> _onAnchorLocalized;

    private void Awake()
    {
        _onAnchorLocalized = OnLocalized;
    }

    private void Update()
    {
        // Botão A -> criar ou restaurar
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            HandleA();
        }

        // Botão B -> remover da cena (mantém UUID em memória)
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            HandleB();
        }
    }

    private void HandleA()
    {
        // Se já existe um componente ativo, nada a fazer
        if (_spatialAnchorComponent != null)
        {
            Debug.Log("[SpatialAnchorController] Âncora já ativa neste objeto.");
            return;
        }

        // Se houver UUID em memória, tentamos carregar / localizar a anchor existente
        if (!string.IsNullOrEmpty(_anchorUuid))
        {
            LoadAndLocalizeByUuid(_anchorUuid);
            return;
        }

        // Caso contrário, criamos um OVRSpatialAnchor neste GameObject e salvamos (async)
        // Fire-and-forget: executa a tarefa assincronamente e evita transformar Update em async.
        _ = CreateAndSaveAnchorAsync();
    }

    private void HandleB()
    {
        // Remove a anchor da cena, mas preserva _anchorUuid em memória para restauração
        if (_spatialAnchorComponent != null)
        {
            // Não removemos do armazenamento persistente do serviço — só tiramos da cena.
            Destroy(_spatialAnchorComponent);
            _spatialAnchorComponent = null;
            Debug.Log("[SpatialAnchorController] Âncora removida da cena. UUID mantido em memória.");
        }
        else
        {
            Debug.Log("[SpatialAnchorController] Nenhuma âncora ativa para remover.");
        }
    }

    // Agora é async Task para podermos aguardar sem corrotinas/yield.
    // Usa polling com `await Task.Yield()` para aguardar até que PendingCreation seja falso
    // (ou até timeout) mantendo a execução no contexto do Unity main thread.
    private async Task CreateAndSaveAnchorAsync()
    {
        // Adiciona componente OVRSpatialAnchor neste GameObject
        _spatialAnchorComponent = gameObject.GetComponent<OVRSpatialAnchor>() ?? gameObject.AddComponent<OVRSpatialAnchor>();

        await _spatialAnchorComponent.WhenLocalizedAsync();
        
        if (_spatialAnchorComponent != null) {
            Debug.Log("[SpatialAnchorController] Componente OVRSpatialAnchor criado e localizado.");
        }

        if (_spatialAnchorComponent)
        {
            if (!_spatialAnchorComponent.Created)
            {
                Debug.LogError("[SpatialAnchorController] Falha ao criar anchor: componente não foi criado com sucesso.");
                return;
            }
            else
            {
                Debug.Log("[SpatialAnchorController] Anchor criada com sucesso. Iniciando SaveAnchorAsync...");
                Debug.Log($"[SpatialAnchorController] Anchor UUID = {_spatialAnchorComponent.Uuid}");
            }
        }
        try
            {
                // SaveAnchorAsync é assíncrono — aguardamos o resultado conforme o SDK
                var saveResult = await _spatialAnchorComponent.SaveAnchorAsync();
                if (saveResult.Success)
                {
                    // Captura UUID disponível no componente (tipo Guid/string conforme SDK)
                    _anchorUuid = _spatialAnchorComponent.Uuid.ToString();
                    Debug.Log($"[SpatialAnchorController] Anchor salva com sucesso. UUID = {_anchorUuid}");
                }
                else
                {
                    Debug.LogError($"[SpatialAnchorController] SaveAnchorAsync falhou: {saveResult.Status}");
                }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SpatialAnchorController] Exceção ao salvar anchor: {ex.Message}");
        }
    }

    private async void LoadAndLocalizeByUuid(string uuid)
    {
        _unboundAnchors.Clear();

        try
        {
            List<Guid> uuids = new List<Guid>();
            uuids.Add(Guid.Parse(uuid));
            // Reutiliza API do SDK para buscar unbound anchors pelo UUID
            var result = await OVRSpatialAnchor.LoadUnboundAnchorsAsync(uuids, _unboundAnchors);
            if (!result.Success)
            {
                Debug.LogError($"[SpatialAnchorController] LoadUnboundAnchorsAsync falhou com {result.Status}");
                return;
            }

            // Processo similar ao sample oficial: iteramos e localizamos se necessário
            foreach (var unbound in result.Value)
            {
                if (unbound.Localized)
                {
                    // Já localizado — bind direto ao componente deste GameObject
                    _onAnchorLocalized(true, unbound);
                }
                else if (!unbound.Localizing)
                {
                    // Inicia localização assíncrona; ao terminar o callback será chamado
                    unbound.LocalizeAsync().ContinueWith(_onAnchorLocalized, unbound);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SpatialAnchorController] Exceção ao carregar/localizar anchor: {ex.Message}");
        }
    }

    // Callback utilizado quando uma UnboundAnchor é localizada (ou já estava localizada).
    // Faz o Bind na `OVRSpatialAnchor` deste GameObject (criando-a se necessário).
    private void OnLocalized(bool success, OVRSpatialAnchor.UnboundAnchor unboundAnchor)
    {
        if (!success)
        {
            Debug.LogError("[SpatialAnchorController] Localization falhou para unbound anchor: " + unboundAnchor);
            return;
        }

        // Garante que exista um componente OVRSpatialAnchor neste GameObject para o Bind
        _spatialAnchorComponent = gameObject.GetComponent<OVRSpatialAnchor>() ?? gameObject.AddComponent<OVRSpatialAnchor>();

        // Faz o bind da UnboundAnchor ao componente (persistente do serviço)
        unboundAnchor.BindTo(_spatialAnchorComponent);

        // Atualiza UUID em memória (útil caso tenha vindo da operação de Load)
        _anchorUuid = unboundAnchor.Uuid.ToString();

        Debug.Log($"[SpatialAnchorController] Anchor localizada e ligada a este objeto. UUID = {_anchorUuid}");
    }
}
