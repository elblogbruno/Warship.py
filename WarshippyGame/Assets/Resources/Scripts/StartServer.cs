 using UnityEngine;
 using System.Collections;
 using System.Diagnostics;
 using System;
 using System.IO;
public class StartServer : MonoBehaviour
{
 // Use this for initialization
     void Start () {
         try {
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = Path.Combine(Application.streamingAssetsPath ,"serverDemozmq.exe");
            p.Start();
            
         } catch (Exception e){
             print(e);        
         }
     }
}
