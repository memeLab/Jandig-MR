
using System.Collections;
using System.IO;
using UnityEngine;
using UnityGLTF;
using UnityEngine.Networking;
using System.Runtime.ExceptionServices;

public class Object3DController : MonoBehaviour
{

    // 3D Object URL
    [SerializeField]
    public string m_url;

    // Start is called before the first frame update
    void Start()
    {
     
        if (m_url != null)
        {
            StartCoroutine(DownloadFile(m_url, Path.GetFileName(m_url)));
        }
        else
        {
            Debug.LogError("3D Object URL is nothing.");
        }
    }

    private IEnumerator DownloadFile(string url, string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            Debug.Log("Downloading 3D Failed. File already exists in the path: " + filePath);
             StartCoroutine(LoadGLBFile(filePath));
            yield break;
        }
        else
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Downloading 3D Failed. File download error: " + www.error);
                }
                else
                {
                    File.WriteAllBytes(filePath, www.downloadHandler.data);
                    Debug.Log("3D File successfully downloaded and saved to: " + filePath);
                    StartCoroutine(LoadGLBFile(filePath));
                }
            }
        }
    }
    private IEnumerator LoadGLBFile(string path)
    {
        Debug.Log("Loading GLB file from path: " + path);
        string filename = Path.GetFileName(path);
        Debug.Log("Filename: " + filename);
        string dir = Path.GetDirectoryName(path);
        Debug.Log("Directory: " + dir);
        var importOpt = new UnityGLTF.ImportOptions();
//#if UNITY_EDITOR
        //importOpt.AnimationMethod = UnityGLTF.AnimationMethod.Mecanim;
//#else
        importOpt.AnimationMethod = UnityGLTF.AnimationMethod.Legacy;
//#endif
        importOpt.DataLoader = new UnityGLTF.Loader.UnityWebRequestLoader(dir);
        Debug.Log("Created ImportOptions and DataLoader.");
        var import = new GLTFSceneImporter(filename, importOpt);
        Debug.Log("Created Import Object");
        Transform parent_transform = this.transform.parent;
        // Função de callback chamada quando a cena for carregada
        System.Action<GameObject, ExceptionDispatchInfo> onLoadComplete = (scene, e) =>
        {
            if (e != null)
            {
                Debug.LogError("Erro ao carregar a cena GLTF: " + e.SourceException.Message);
                return;
            }
            Debug.Log("Cena GLTF carregada com sucesso.");
            // Define o mesmo parent do GameObject que possui este script
            scene.transform.SetParent(parent_transform, worldPositionStays: false);
//#if !UNITY_EDITOR
            Debug.Log("Playing Animation manually on legacy mode.");
            Animation anim = scene.GetComponent <Animation>();
            anim.Play();
//#endif
            Debug.Log("Objeto 3d Posicionado com sucesso");
        };         
        StartCoroutine(import.LoadSceneAsync(onLoadComplete: onLoadComplete).AsCoroutine());
        yield break;
    }

 
}
