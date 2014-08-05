using UnityEngine;
using LitJson;

public abstract class JsonHandler{
	
	abstract public void Write(string file);
	abstract public void Read(string file);
}
