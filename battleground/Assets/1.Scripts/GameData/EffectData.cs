using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.IO;

/// <summary>
/// 이팩트 클립 리스트와 이팩트 파일 이름과 경로를 가지고 있으며
/// 파일을 읽고 쓰는 기능을 갖고 있다.
/// </summary>
public class EffectData : BaseData {
    public EffectClip[] effectClips = new EffectClip[0];
    
    public string clipPath = "Effects/";
    private string xmlFilePath = "";
    private string xmlFileName = "effectData.xml";
    private string dataPath = "Data/effectData";
    private const string EFFECT = "effect";
    private const string CLIP = "clip";
    
    private EffectData() {}
    // 읽어오고, 저장하고, 데이터를 삭제하고, 특정 클립을 가져오고, 복사하는 기능
    
    
    public void LoadData() {
        this.xmlFilePath = Application.dataPath + dataDirectory;
        TextAsset asset = (TextAsset)ResourceManager.Load(dataPath);
        if (asset == null || asset.text == null) {
            this.AddData("New Effect");
            return;
        }

        using (XmlTextReader reader = new XmlTextReader(new StringReader(asset.text))) {
            int currentId = 0;
            while (reader.Read()) {
                if (reader.IsStartElement()) {
                    switch (reader.Name) {
                        case "length":
                            int length = int.Parse(reader.ReadString());
                            this.names = new string[length];
                            this.effectClips = new EffectClip[length];
                            break;
                        case "id":
                            currentId = int.Parse(reader.ReadString());
                            this.effectClips[currentId] = new EffectClip();
                            this.effectClips[currentId].realId = currentId;
                            break;
                        case "name":
                            this.names[currentId] = reader.ReadString();
                            break;
                        case "effectType":
                            this.effectClips[currentId].effectType =
                                (EffectType)Enum.Parse(typeof(EffectType), reader.ReadString());
                            break;
                        case "effectName":
                            this.effectClips[currentId].effectName = reader.ReadString();
                            break;
                        case "effectPath":
                            this.effectClips[currentId].effectPath = reader.ReadString();
                            break;
                    }
                }
            }
        }
    }

    public void SaveData() {
        using (XmlTextWriter xml = new XmlTextWriter(xmlFilePath + xmlFileName, System.Text.Encoding.Unicode)) {
            xml.WriteStartDocument();
            xml.WriteStartElement(EFFECT);
            xml.WriteElementString("length", GetDataCount().ToString());
            for (int i = 0; i < this.names.Length; i++) {
                EffectClip clip = this.effectClips[i];
                xml.WriteStartElement(CLIP);
                xml.WriteElementString("id", i.ToString());
                xml.WriteElementString("name", this.names[i]);
                xml.WriteElementString("effectType", clip.effectType.ToString());
                xml.WriteElementString("effectPath", clip.effectPath);
                xml.WriteElementString("effectName", clip.effectName);
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
            xml.WriteEndDocument();
        }
    }

    public override int AddData(string newName) {
        if (this.names == null) {
            this.names = new string[] { name };
            this.effectClips = new EffectClip[] { new EffectClip() };
        } else {
            this.names = ArrayHelper.Add(name, this.names);
            this.effectClips = ArrayHelper.Add(new EffectClip(), this.effectClips);
        }

        return GetDataCount();
    }

    public override void RemoveData(int index) {
        this.names = ArrayHelper.Remove(index, this.names);
        if (this.names.Length == 0) {
            this.names = null;
        }

        this.effectClips = ArrayHelper.Remove(index, this.effectClips);
    }

    public void ClearData() {
        foreach (EffectClip clip in this.effectClips) {
            clip.ReleaseEffect();
        }

        this.effectClips = null;
        this.names = null;
    }

    public EffectClip GetCopy(int index) {
        if (index < 0 || index >= this.effectClips.Length) {
            return null;
        }

        EffectClip original = this.effectClips[index];
        EffectClip clip = new EffectClip();
        clip.effectFullPath = original.effectFullPath;
        clip.effectName = original.effectName;
        clip.effectType = original.effectType;
        clip.effectPath = original.effectPath;
        clip.realId = this.effectClips.Length;
        return clip;
    }

    public EffectClip GetClip(int index) {
        if (index < 0 || index >= this.effectClips.Length) {
            return null;
        }
        effectClips[index].PreLoad();
        return effectClips[index];
    }
    
    public override void CopyData(int index) {
        this.names = ArrayHelper.Add(this.names[index], this.names);
        this.effectClips = ArrayHelper.Add(GetCopy(index), this.effectClips);
    }
}
