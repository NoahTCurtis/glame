using UnityEngine;
using System.Collections;

using System.Threading;

namespace ZKAPI{
	[AddComponentMenu("")]
	public class ThreadedAutomator : MonoBehaviour{
		[Range(1f,5000f)]
		public int latency = 1000;
		bool isInitialized = false;
		protected ChunkObject volume;
		Thread mainThread;
		volatile bool mustRemesh;
		volatile string consoleOut;
		void Awake(){
			volume = GetComponent<ChunkObject>();
			if(volume == null){
				enabled = false;
			}else{
				if(enabled && volume.enabled){
					Debug.Log ("Starting automator thread...");
					mainThread = new Thread(new ThreadStart(this.ThreadedAutomation));
					mainThread.Start();
					Debug.Log ("Thread has started.");
				}
			}
		}
		#region threaded
		void ThreadedAutomation(){
			while(!isInitialized){}
			Vector3 volumeSize = volume.GetSize();
			mustRemesh = Once();
			Debug.Log ("This happens once.");
			while(true){
				if(Loop()){
					mustRemesh = true;
				}
				for(int x =0; x<volumeSize.x;x++){
					for(int y =0; y<volumeSize.y; y++){
						for(int z =0; z<volumeSize.z; z++){
							if(isInitialized){
								if(PerVoxel(new Vector3(x,y,z))){
									mustRemesh = true;
								}
							}
						}
					}
				}
				Thread.Sleep(latency);
			}
		}
		protected virtual bool Once(){
			return false;
		}
		protected virtual bool Loop(){
			return false;
		}
		protected virtual bool PerVoxel(Vector3 position){
			return false;
		}

		protected void WriteLine(string message, bool newline=true){
			if(newline){
				consoleOut = consoleOut+"\n";
			}
			consoleOut = consoleOut+message;
		}
		#endregion
		#region Synced
		void Update(){
			if(isInitialized){
				if(mustRemesh){
					volume.ReMesh();
					mustRemesh = false;
				}
				if(consoleOut != null){
					Debug.Log (consoleOut);
					consoleOut = null;
				}
				if(volume == null){
					isInitialized = false;
					mainThread.Abort();
				}
			}else{
				if(volume.Lookup()!= null){
					isInitialized = true;
				}
			}
			if(!volume.enabled){
				this.enabled = false;
			}
		}
		#endregion
		void OnDestroy(){
			if(mainThread != null){
				mainThread.Abort();
				mainThread.Join();
			}
		}
		void OnApplicationQuit(){
			if(mainThread != null){
				mainThread.Abort();
				mainThread.Join();
			}
		}
		void OnDisable(){
			if(mainThread != null){
				mainThread.Abort();
				mainThread.Join();
			}
		}
		void OnEnable(){
			if(mainThread == null && volume.enabled){
				Debug.Log ("Starting automator thread...");
				mainThread = new Thread(new ThreadStart(this.ThreadedAutomation));
				mainThread.Start();
				Debug.Log ("Thread has started.");
			}
		}
	}
}
