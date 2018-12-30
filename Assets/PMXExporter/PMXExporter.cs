using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MMDataIO.Pmx;
using System.IO;
using VRM;

#if UNITY_EDITOR
using UnityEditor;
#endif
 
[ExecuteInEditMode]
public class PMXExporter : MonoBehaviour
{
    public bool ReplaceBoneName = true;
    public bool ConvertArmatuar_NeedReplaceBoneName = true;
    public bool ReplaceMorphName = true;

    public bool useReverseJoint = false;
    public bool emulateSpringBoneLeafColider = false;

    public bool useAntiGravity = false;
    public bool boneDirModify = true;

    public float caScaling = 14.0f;

#if UNITY_EDITOR

    public enum RenderMode
    {
        Opaque,
        Cutout,
        Transparent
    }
    public enum OutlineWidthMode
    {
        None,
        WorldCoordinates,
        ScreenCoordinates
    }

    [ContextMenu("Export")]
    void Init()
    {
        Debug.Log("build 258");


        bool ConvertArmatuar = ReplaceBoneName && ConvertArmatuar_NeedReplaceBoneName;

        Quaternion? tmpRootRot = null;
        Quaternion? tmpLUARot = null;
        Quaternion? tmpRUARot = null;

        try
        {
            if (ConvertArmatuar)
            {
                var anim = GetComponent<Animator>();
                var lua = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                tmpLUARot = lua.rotation;
                lua.Rotate(new Vector3(0, 0, 30));

                var rua = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
                tmpRUARot = rua.rotation;
                rua.Rotate(new Vector3(0, 0, -30));

                tmpRootRot = transform.rotation;
                transform.Rotate(new Vector3(0, 180, 0));
                transform.localScale *= caScaling;
            }


            var filterdFileName = System.IO.Path.GetInvalidFileNameChars().Aggregate(
                name, (s, c) => s.Replace(c.ToString(), ""));
            string savepath = EditorUtility.SaveFilePanel("Export PMX", "", filterdFileName, "pmx");

            if (string.IsNullOrEmpty(savepath))
            {
                Debug.Log("SavePath not exists.");
                return;
            }

            var pmx = new PmxModelData();

            using (FileStream fs = new FileStream("Assets/PMXExporter/vanilla.pmx", FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs))
            {
                pmx.Read(br);
            }
            Debug.Log("Load vanilla.pmx");


            var ikTemplate = new PmxModelData();

            using (FileStream fs = new FileStream("Assets/PMXExporter/iktemplate.pmx", FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs))
            {
                ikTemplate.Read(br);
            }
            Debug.Log("Load iktemplate.pmx");


            var slots = new List<PmxSlotData>();

            #region build_bone
            {//bone

                var humanoid2mmd = new SortedDictionary<HumanBodyBones, string>();
                {
                    humanoid2mmd.Add(HumanBodyBones.Hips, "下半身");
                    humanoid2mmd.Add(HumanBodyBones.Spine, "上半身");
                    humanoid2mmd.Add(HumanBodyBones.Chest, "上半身2");
                    humanoid2mmd.Add(HumanBodyBones.LeftUpperLeg, "左足");
                    humanoid2mmd.Add(HumanBodyBones.LeftLowerLeg, "左ひざ");
                    humanoid2mmd.Add(HumanBodyBones.LeftFoot, "左足首");
                    humanoid2mmd.Add(HumanBodyBones.LeftToes, "左つま先");
                    humanoid2mmd.Add(HumanBodyBones.RightUpperLeg, "右足");
                    humanoid2mmd.Add(HumanBodyBones.RightLowerLeg, "右ひざ");
                    humanoid2mmd.Add(HumanBodyBones.RightFoot, "右足首");
                    humanoid2mmd.Add(HumanBodyBones.RightToes, "右つま先");
                    humanoid2mmd.Add(HumanBodyBones.Neck, "首");
                    humanoid2mmd.Add(HumanBodyBones.Head, "頭");
                    humanoid2mmd.Add(HumanBodyBones.LeftEye, "左目");
                    humanoid2mmd.Add(HumanBodyBones.RightEye, "右目");
                    humanoid2mmd.Add(HumanBodyBones.LeftShoulder, "左肩");
                    humanoid2mmd.Add(HumanBodyBones.LeftUpperArm, "左腕");
                    humanoid2mmd.Add(HumanBodyBones.LeftLowerArm, "左ひじ");
                    humanoid2mmd.Add(HumanBodyBones.LeftHand, "左手首");
                    humanoid2mmd.Add(HumanBodyBones.RightShoulder, "右肩");
                    humanoid2mmd.Add(HumanBodyBones.RightUpperArm, "右腕");
                    humanoid2mmd.Add(HumanBodyBones.RightLowerArm, "右ひじ");
                    humanoid2mmd.Add(HumanBodyBones.RightHand, "右手首");
                    humanoid2mmd.Add(HumanBodyBones.LeftRingProximal, "左薬指１");
                    humanoid2mmd.Add(HumanBodyBones.LeftRingIntermediate, "左薬指２");
                    humanoid2mmd.Add(HumanBodyBones.LeftRingDistal, "左薬指３");

                    humanoid2mmd.Add(HumanBodyBones.LeftThumbProximal, "左親指０");
                    humanoid2mmd.Add(HumanBodyBones.LeftThumbIntermediate, "左親指１");
                    humanoid2mmd.Add(HumanBodyBones.LeftThumbDistal, "左親指２");

                    humanoid2mmd.Add(HumanBodyBones.LeftIndexProximal, "左人指１");
                    humanoid2mmd.Add(HumanBodyBones.LeftIndexIntermediate, "左人指２");
                    humanoid2mmd.Add(HumanBodyBones.LeftIndexDistal, "左人指３");
                    humanoid2mmd.Add(HumanBodyBones.LeftMiddleProximal, "左中指１");
                    humanoid2mmd.Add(HumanBodyBones.LeftMiddleIntermediate, "左中指２");
                    humanoid2mmd.Add(HumanBodyBones.LeftMiddleDistal, "左中指３");
                    humanoid2mmd.Add(HumanBodyBones.LeftLittleProximal, "左小指１");
                    humanoid2mmd.Add(HumanBodyBones.LeftLittleIntermediate, "左小指２");
                    humanoid2mmd.Add(HumanBodyBones.LeftLittleDistal, "左小指３");
                    humanoid2mmd.Add(HumanBodyBones.RightRingProximal, "右薬指１");
                    humanoid2mmd.Add(HumanBodyBones.RightRingIntermediate, "右薬指２");
                    humanoid2mmd.Add(HumanBodyBones.RightRingDistal, "右薬指３");

                    humanoid2mmd.Add(HumanBodyBones.RightThumbProximal, "右親指０");
                    humanoid2mmd.Add(HumanBodyBones.RightThumbIntermediate, "右親指１");
                    humanoid2mmd.Add(HumanBodyBones.RightThumbDistal, "右親指２");

                    humanoid2mmd.Add(HumanBodyBones.RightIndexProximal, "右人指１");
                    humanoid2mmd.Add(HumanBodyBones.RightIndexIntermediate, "右人指２");
                    humanoid2mmd.Add(HumanBodyBones.RightIndexDistal, "右人指３");
                    humanoid2mmd.Add(HumanBodyBones.RightMiddleProximal, "右中指１");
                    humanoid2mmd.Add(HumanBodyBones.RightMiddleIntermediate, "右中指２");
                    humanoid2mmd.Add(HumanBodyBones.RightMiddleDistal, "右中指３");
                    humanoid2mmd.Add(HumanBodyBones.RightLittleProximal, "右小指１");
                    humanoid2mmd.Add(HumanBodyBones.RightLittleIntermediate, "右小指２");
                    humanoid2mmd.Add(HumanBodyBones.RightLittleDistal, "右小指３");

                }
                var humanoidMapping = new SortedDictionary<string, HumanBodyBones>();
                var humanoidDesc = GetComponent<VRM.VRMHumanoidDescription>();


                foreach (var x in humanoidDesc.Description.human)
                {
                    humanoidMapping.Add(x.boneName, x.humanBone);
                }

                System.Func<string, string> remapBoname = (orgName) =>
                {
                    string remapName = orgName;

                    if (humanoidMapping.ContainsKey(orgName))
                    {
                        HumanBodyBones key = humanoidMapping[orgName];

                        if (humanoid2mmd.ContainsKey(key))
                        {
                            remapName = humanoid2mmd[key];
                        }
                    }

                    return remapName;
                };

                if (!ReplaceBoneName)
                {
                    remapBoname = (orgName) => orgName;
                }


                var bones = new List<Transform>();
                var boneStack = new Stack<Transform>();


                boneStack.Push(transform);
                while (0 < boneStack.Count)
                {
                    var obj = boneStack.Pop();
                    bones.Add(obj);

                    foreach (Transform child in obj.transform)
                    {
                        boneStack.Push(child);
                    }
                }

                var indexedBones = bones.Select((b, idx) => new { id = idx, bone = b }).ToArray();

                var create = indexedBones.Select(b => new PmxBoneData()
                {
                    BoneName = remapBoname(b.bone.name),
                    BoneNameE = b.bone.name,
                    BoneId = b.id,
                    Pos = b.bone.position,
                    ParentId = b.bone.parent == null ? -1 : ((
                        indexedBones.Where(elm => elm.bone.name == b.bone.parent.name).FirstOrDefault()
                        ?? indexedBones.First()
                        ).id),
                    Flag = BoneFlags.VISIBLE | BoneFlags.ROTATE | BoneFlags.OP,
                    PosOffset = new Vector3(0, 0, ConvertArmatuar ? -0.5f : 0.1f),
                });

                pmx.BoneArray = create.ToArray();



                if (boneDirModify) //ボーンの向きを変える（ローカル軸ではない
                {
                    foreach (PmxBoneData bone in pmx.BoneArray)
                    {
                        var targetId = bone.BoneId;

                        var children = pmx.BoneArray.Where(x => x.ParentId == targetId);

                        var cCount = children.Count();

                        if (cCount == 0)
                        {
                            if (0 < bone.ParentId)
                            {

                                var parent = pmx.BoneArray[bone.ParentId];

                                Vector3 comPos = bone.Pos - parent.Pos;

                                if (caScaling < Vector3.Magnitude(comPos) || emulateSpringBoneLeafColider)
                                {
                                    comPos /= Vector3.Magnitude(comPos);
                                    comPos *= caScaling;
                                }

                                comPos *= 0.7f;

                                bone.PosOffset = comPos;
                            }
                            else
                                continue;
                        }
                        else
                        {
                            var offsets = children
                                .Select(x => (x.Pos - bone.Pos))
                                .Where(offset => 0.1 < Vector3.Magnitude(offset));

                            var sumOffset = offsets
                                .Aggregate(Vector3.zero, (avg, elem) => avg + elem);

                            var avgOffset = sumOffset / offsets.Count();

                            bone.PosOffset = avgOffset;
                        }

                        if (Vector3.Magnitude(bone.PosOffset) < 0.001f)
                        {
                            bone.PosOffset = Vector3.Normalize(bone.PosOffset);
                            if (Vector3.Magnitude(bone.PosOffset) == 0)
                                bone.PosOffset = new Vector3(0, 0, -1.0f);
                        }
                    }
                }


                if (ConvertArmatuar)
                {
                    Debug.Log("ConvertArmatuar");
                    int centerId = -1;

                    var hips = pmx.BoneArray.Where(b => b.BoneName.Equals("下半身")).FirstOrDefault();
                    if (hips != null && 0 <= hips.ParentId && hips.ParentId < pmx.BoneArray.Length)
                    {
                        centerId = hips.ParentId;

                        var centerBone = pmx.BoneArray[centerId];
                        centerBone.BoneName = "センター";
                        centerBone.Flag |= BoneFlags.MOVE;

                        pmx.BoneArray[0].BoneName = "全ての親";
                        pmx.BoneArray[0].Flag |= BoneFlags.MOVE;
                    }

                    var spine = pmx.BoneArray.Where(b => b.BoneName.Equals("上半身")).FirstOrDefault();
                    if (0 <= centerId && spine != null)
                    {
                        spine.ParentId = centerId;
                    }


                    var bonesList = new List<PmxBoneData>(pmx.BoneArray);


                    System.Func<string, string, int, int> addIk = (ikname, targetname, parentid) =>
                   {
                       int result = -1;
                       var ik = ikTemplate.BoneArray.Where(b => b.BoneName.Equals(ikname)).FirstOrDefault();
                       if (ik != null)
                       {
                           ik = (PmxBoneData)ik.Clone();

                           result = ik.BoneId = bonesList.Count();

                           ik.ParentId = parentid;

                           var target = bonesList.Where(b => b.BoneName.Equals(targetname)).FirstOrDefault();
                           if (target != null)
                           {
                               ik.IkTargetId = target.BoneId;

                               ik.Pos = target.Pos;
                           }

                           for (int i = 0; i < ik.IkChilds.Length; i++) {

                               var name = ikTemplate.BoneArray[ik.IkChilds[i].ChildId].BoneName;
                               Debug.Log("ik child name : " + name);


                               var childbone = bonesList.Where(b => b.BoneName.Equals(name)).FirstOrDefault();
                               Debug.Log("ik child name : " + name);

                               if (childbone != null)
                               {
                                   ik.IkChilds[i].ChildId = childbone.BoneId;
                               }
                           }
                           bonesList.Add(ik);
                       }

                       return result;
                   };

                    {
                        int pid = addIk("左足ＩＫ", "左足首", 0);
                        if (0 < pid)
                            addIk("左つま先ＩＫ", "左つま先", pid);
                    }

                    {
                        int pid = addIk("右足ＩＫ", "右足首", 0);
                        if (0 < pid)
                            addIk("右つま先ＩＫ", "右つま先", pid);
                    }

                    //*

                    {
                        var head = bonesList.Where(b => b.BoneName.Equals("頭")).FirstOrDefault();

                        var eyes = new PmxBoneData()
                        {
                            BoneName = "両目",
                            BoneNameE = "eyes",
                            BoneId = bonesList.Count(),
                            Pos = new Vector3(0, head.Pos.y * 1.5f, 0),
                            ParentId = head.BoneId,
                            Flag = BoneFlags.VISIBLE | BoneFlags.ROTATE | BoneFlags.OP,
                            PosOffset = new Vector3(0, 0, -1.0f),
                        };

                        bonesList.Add(eyes);


                        var eyeBones = bonesList.Where(b => b.BoneName.Equals("左目") || b.BoneName.Equals("右目"));
                        foreach (PmxBoneData bone in eyeBones)
                        {
                            bone.LinkParentId = eyes.BoneId;
                            bone.LinkWeight = 1.0f;
                            bone.Flag |= BoneFlags.ROTATE_LINK;
                        }


                        //transform level set
                        {
                            var legsStack = new Stack<PmxBoneData>(bonesList.FindAll(b => b.BoneName.Equals("右足") || b.BoneName.Equals("左足")));
                            while(0 < legsStack.Count())
                            {
                                var bone = legsStack.Pop();

                                if (!humanoid2mmd.ContainsValue(bone.BoneName))
                                {
                                    bone.Depth = 2;
                                }


                                var childs = bonesList.FindAll(b => b.ParentId == bone.BoneId);
                                foreach (var child in childs)
                                    legsStack.Push(child);
                            }
                        }
                    }

                    {//add 捩bone


                        System.Func<string, string, string, PmxBoneData> addTwistBone = (entryBoneName, dirBoneName, addBoneName) =>
                        {
                            var entryBone = bonesList.Where(b => b.BoneName.Equals(entryBoneName)).FirstOrDefault();
                            var dirBone = bonesList.Where(b => b.BoneName.Equals(dirBoneName)).FirstOrDefault();

                            var twistBone = new PmxBoneData()
                            {
                                BoneName = addBoneName,
                                BoneNameE = entryBone.BoneName + "_twist",
                                BoneId = bonesList.Count(),
                                ParentId = entryBone.BoneId,

                                Pos = (entryBone.Pos + dirBone.Pos) / 2.0f,
                                AxisVec = Vector3.Normalize(entryBone.Pos - dirBone.Pos),

                                Depth = entryBone.Depth,

                                Flag = BoneFlags.VISIBLE | BoneFlags.ROTATE | BoneFlags.AXIS_ROTATE | BoneFlags.OP,
                                PosOffset = new Vector3(0, 0, -1.0f),
                            };

                            bonesList.Add(twistBone);

                            //depth increment
                            {
                                dirBone.ParentId = twistBone.BoneId;

                                var depthIncStack = new Stack<PmxBoneData>();
                                depthIncStack.Push(dirBone);

                                var newDepth = twistBone.Depth + 1;
                                while (0 < depthIncStack.Count())
                                {
                                    var bone = depthIncStack.Pop();
                                    bone.Depth = newDepth;

                                    var childs = bonesList.FindAll(b => b.ParentId == bone.BoneId);
                                    foreach (var child in childs)
                                        depthIncStack.Push(child);
                                }
                            }

                            return twistBone;
                        };

                        addTwistBone("右腕", "右ひじ", "右腕捩");
                        addTwistBone("左腕", "左ひじ", "左腕捩");

                        addTwistBone("右ひじ", "右手首", "右手捩");
                        addTwistBone("左ひじ", "左手首", "左手捩");
                    }

                    pmx.BoneArray = bonesList.ToArray();

                    /**/
                }

            }

            #endregion



            {//vertex face blendshape
                var vertices = new List<Vector3>();
                var normals = new List<Vector3>();
                var uvs = new List<Vector2>();

                var renderers = new List<Renderer>();
                var boneWeights = new List<BoneWeight>();

                var faces = new List<int[]>();
                var mattex = new List<KeyValuePair<string, string>>();
                var subtex = new List<string>();
                var materials = new List<Material>();

                var morphs = new List<PmxMorphData>();

                var orgRenderers = GetComponentsInChildren<Renderer>();
                //GetComponentsInChildren<MeshFilter>(true);

                var Offsets = new int[orgRenderers.Length];

                System.Func<IList, string> getCountLog = (x) => x.ToString() + "count = " + x.Count;

                Debug.Log("mesh count:" + orgRenderers.Length);

                int vertexCount = 0;
                for (int i = 0; i < orgRenderers.Length; i++)
                {
                    //Debug.Log("LoopCount : " + i);
                    Debug.Log("MeshNames : " + orgRenderers[i].gameObject.name);


                    var blendshapeMapping = new SortedDictionary<string, string>();
                    var blendshapeMappingSlot = new SortedDictionary<string, MorphSlotType>();
                    using (var fs = new FileStream("Assets/PMXExporter/BlendShapeRenameMap.csv", FileMode.Open))
                    using (var reader = new System.IO.StreamReader(fs))
                    {
                        while (reader.Peek() != -1) // reader.Peaekが-1になるまで
                        {
                            var entry = reader.ReadLine().Split(',');
                            if (2 <= entry.Length)
                            {
                                if (!blendshapeMapping.ContainsKey(entry[0]))
                                    blendshapeMapping.Add(entry[0], entry[1]);
                            }

                            if(3 <= entry.Length)
                            {
                                if (!blendshapeMappingSlot.ContainsKey(entry[0]))
                                {
                                    var typeText = entry[2];
                                    var type = MorphSlotType.RIP; //def その他
                                    if (typeText.Equals("E"))
                                        type = MorphSlotType.EYE;
                                    else if (typeText.Equals("B"))
                                        type = MorphSlotType.EYEBROW;
                                    else if (typeText.Equals("M"))
                                        type = MorphSlotType.MOUTH;

                                    blendshapeMappingSlot.Add(entry[0], type);
                                }
                            }
                        }
                    }
                    Debug.Log("BlendShapeRenameMap.csv");

                    System.Func<string, string> remapMorphName = (orgName) =>
                    {
                        string remapName = orgName;

                        if (blendshapeMapping.ContainsKey(orgName))
                        {
                            remapName = blendshapeMapping[orgName];
                        }

                        return remapName;
                    };

                    if (!ReplaceMorphName)
                    {
                        remapMorphName = (orgName) => orgName;
                    }

                    System.Func<string, MorphSlotType> remapMorphType = (orgName) =>
                    {
                        MorphSlotType remapName = MorphSlotType.RIP;

                        if (blendshapeMappingSlot.ContainsKey(orgName))
                        {
                            remapName = blendshapeMappingSlot[orgName];
                        }

                        return remapName;
                    };

                    var blendProxyMapping = new SortedDictionary<string, string>();
                    var blendProxyMappingSlot = new SortedDictionary<string, MorphSlotType>();
                    using (var fs = new FileStream("Assets/PMXExporter/BlendProxyRenameMap.csv", FileMode.Open))
                    using (var reader = new System.IO.StreamReader(fs))
                    {
                        while (reader.Peek() != -1) // reader.Peaekが-1になるまで
                        {
                            var entry = reader.ReadLine().Split(',');
                            if (2 <= entry.Length)
                            {
                                if (!blendProxyMapping.ContainsKey(entry[0]))
                                    blendProxyMapping.Add(entry[0], entry[1]);
                            }

                            if (3 <= entry.Length)
                            {
                                if (!blendProxyMappingSlot.ContainsKey(entry[0]))
                                {
                                    var typeText = entry[2];
                                    var type = MorphSlotType.RIP; //def その他
                                    if (typeText.Equals("E"))
                                        type = MorphSlotType.EYE;
                                    else if (typeText.Equals("B"))
                                        type = MorphSlotType.EYEBROW;
                                    else if (typeText.Equals("M"))
                                        type = MorphSlotType.MOUTH;

                                    blendProxyMappingSlot.Add(entry[0], type);
                                }
                            }
                        }
                    }
                    Debug.Log("BlendProxyRenameMap.csv");

                    System.Func<string, string> remapProxyName = (orgName) =>
                    {
                        string remapName = orgName;

                        if (blendProxyMapping.ContainsKey(orgName))
                        {
                            remapName = blendProxyMapping[orgName];
                        }

                        return remapName;
                    };

                    System.Func<string, MorphSlotType> remapProxyMorphType = (orgName) =>
                    {
                        MorphSlotType remapName = MorphSlotType.RIP;

                        if (blendProxyMappingSlot.ContainsKey(orgName))
                        {
                            remapName = blendProxyMappingSlot[orgName];
                        }

                        return remapName;
                    };


                    Mesh mesh = null;
                    var renderer = orgRenderers[i];

                    bool isSkinned;
                    if (renderer is SkinnedMeshRenderer)
                    {
                        isSkinned = true;

                        Mesh srcMesh = ((SkinnedMeshRenderer)renderer).sharedMesh;
                        mesh = srcMesh.Copy(true);
                        ((SkinnedMeshRenderer)renderer).BakeMesh(mesh);

                        mesh.boneWeights = srcMesh.boneWeights; // restore weights. clear when BakeMesh
                                                                // recalc bindposes

                        mesh.bindposes = srcMesh.bindposes.ToArray();
                        //mesh.bindposes = bones.Select(x => x.worldToLocalMatrix * dst.transform.localToWorldMatrix).ToArray();

                        //var m = src.localToWorldMatrix; // include scaling
                        var m = default(Matrix4x4);
                        m.SetTRS(Vector3.zero, transform.rotation, Vector3.one); // rotation only
                        mesh.ApplyMatrix(m);
                        




                        //mesh = ((SkinnedMeshRenderer)renderer).sharedMesh;
                    }
                    else if (renderer is MeshRenderer)
                    {
                        isSkinned = false;
                        var mfilter = renderer.GetComponent<MeshFilter>();
                        //mesh = mfilter.sharedMesh;
                        
                        mesh = mfilter.sharedMesh.Copy(false);
                        mesh.ApplyMatrix(renderer.gameObject.transform.localToWorldMatrix);
                        //mesh.vertices = mesh.vertices.Select(v => renderer.gameObject.transform.localToWorldMatrix.MultiplyVector(v)).ToArray();

                    }
                    else
                        continue;

                    //vertex

                    Offsets[i] = vertexCount;

                    vertexCount += mesh.vertexCount;

                    vertices.AddRange(mesh.vertices);
                    normals.AddRange(mesh.normals);
                    uvs.AddRange(mesh.uv.Select(x => new Vector2() { x = x.x, y = -x.y }));

                    renderers.AddRange(mesh.vertices.Select(v => renderer));


                    var count = mesh.boneWeights != null ? mesh.boneWeights.Length : 0;

                    boneWeights.AddRange(count != 0
                        ? mesh.boneWeights
                        : mesh.vertices.Select(v => new BoneWeight() { boneIndex0 = -1, weight0 = 1 })
                        );

                    //Debug.Log("mesh concat : " + getCountLog(boneWeights));

                    //face

                    if (isSkinned)
                        for (int subIdx = 0; subIdx < renderer.sharedMaterials.Length; subIdx++)
                            faces.Add(mesh.GetTriangles(subIdx, true).Select(tri => Offsets[i] + tri).ToArray());
                    else
                        faces.Add(mesh.triangles.Select(x => x + Offsets[i]).ToArray());

                    materials.AddRange(renderer.sharedMaterials);

                    Debug.Log(renderer.sharedMaterials);
                    //materials
                    Debug.Log("MaterialCount : " + renderer.sharedMaterials.Length);
                    //Debug.Log("MaterialCount : " + renderer.sharedMaterials.Length);
                    mattex.AddRange(renderer.sharedMaterials.Select(x =>
                    {
                        /*
                        Debug.Log("Textures");
                        x.GetTexturePropertyNames().Select(t => { Debug.Log(t); return t; }).ToArray();
                        */
                        return new KeyValuePair<string, string>(
                            x.name
                            , x.GetTexture("_MainTex") != null ? x.GetTexture("_MainTex").name : ""
                            );
                    }));
                    subtex.AddRange(renderer.sharedMaterials.Where(x => x.HasProperty("_SphereAdd")).Select(x =>
                    {
                        /*
                        Debug.Log("Textures");
                        x.GetTexturePropertyNames().Select(t => { Debug.Log(t); return t; }).ToArray();
                        */
                        return x.GetTexture("_SphereAdd") != null ? x.GetTexture("_SphereAdd").name : "";
                    }));


                    //morph

                    var blendShapes = Enumerable.Range(0, mesh.blendShapeCount).Select(idx =>
                    {
                        var name = mesh.GetBlendShapeName(idx);

                        var bpos = new Vector3[mesh.vertexCount];
                        var bnol = new Vector3[mesh.vertexCount];
                        var btan = new Vector3[mesh.vertexCount];

                        var maxframe = mesh.GetBlendShapeFrameCount(idx);
                        mesh.GetBlendShapeFrameVertices(idx, maxframe - 1, bpos, bnol, btan);

                        var m = transform.localToWorldMatrix;
                        m.SetColumn(3, new Vector4(0, 0, 0, 1)); // remove translation
                        var srBpos = bpos.Select(x => m.MultiplyPoint(x)).ToArray();

                        var blendPos = srBpos.Select((pos, deltaIdx) => new { vertexIndex = Offsets[i] + deltaIdx, delta = pos}).ToArray();
                        return new { blendIdx = idx, name = name, blendPos = blendPos };
                       
                    });

                    morphs.AddRange(
                        blendShapes.Select(x => new PmxMorphData()
                        {
                            MorphName = remapMorphName(x.name),
                            MorphNameE = x.name,
                            MorphType = MorphType.VERTEX,
                            MorphArray = (x.blendPos.Select(pos => (IPmxMorphTypeData)new PmxMorphVertexData()
                            {
                                Index = pos.vertexIndex,
                                Position = pos.delta
                            }).ToArray()),
                            SlotType = remapMorphType(x.name),
                        }).ToArray());

                    {
                        var proxy = GetComponent<VRMBlendShapeProxy>();

                        if (proxy.BlendShapeAvatar == null) continue;

                        var clips = proxy.BlendShapeAvatar.Clips;

                        foreach (var clip in clips)
                        {
                            var dataList = new List<IPmxMorphTypeData>();

                            foreach (var blendshape in clip.Values)
                            {
                                //Debug.Log("BlendShapePath : " + blendshape.ToString());

                                var target = transform.Find(blendshape.RelativePath);
                                if (target != null)
                                {
                                    var sr = target.GetComponent<SkinnedMeshRenderer>();
                                    if (sr != null)
                                    {
                                        var blendshapeName = sr.sharedMesh.GetBlendShapeName(blendshape.Index);

                                        dataList.Add(new PmxMorphGroupData()
                                        {
                                            Index = morphs.FindIndex(x => x.MorphNameE.Equals(blendshapeName)),
                                            Weight = blendshape.Weight / 100.0f,
                                        });
                                    }
                                }
                            }

                            /*
                            foreach (var materialblend in clip.MaterialValues)
                            {
                                Debug.Log("MaterialBlend : " + materialblend.ToString());
                            }
                            */
                            if (0 == dataList.Count())
                                continue;

                            if(1 == dataList.Count())
                            {
                                var morphData = morphs[dataList[0].Index];
                                morphData.MorphName = remapProxyName(clip.BlendShapeName);
                                morphData.SlotType = remapProxyMorphType(clip.BlendShapeName);
                            }
                            else
                            {
                                morphs.Add(new PmxMorphData()
                                {
                                    MorphName = remapProxyName(clip.BlendShapeName),
                                    MorphNameE = clip.BlendShapeName,
                                    MorphType = MorphType.GROUP,
                                    MorphArray = dataList.ToArray(),
                                    SlotType = remapProxyMorphType(clip.BlendShapeName),
                                });
                            }

                        }

                    }
                }

                #region build 


                Debug.Log(getCountLog(vertices));
                Debug.Log(getCountLog(normals));
                Debug.Log(getCountLog(uvs));
                Debug.Log(getCountLog(renderers));
                Debug.Log(getCountLog(boneWeights));

                Debug.Log(getCountLog(mattex));
                Debug.Log(getCountLog(subtex));

                #region build_vertex

                var buildVertices = new List<PmxVertexData>();
                foreach (var pos in vertices.Select((vertex, index) => new { vertex, index }))
                {
                    var boneWeightOrg = new[]{
                    new {id = boneWeights[pos.index].boneIndex0 , weight = boneWeights[pos.index].weight0},
                    new {id = boneWeights[pos.index].boneIndex1 , weight = boneWeights[pos.index].weight1},
                    new {id = boneWeights[pos.index].boneIndex2 , weight = boneWeights[pos.index].weight2},
                    new {id = boneWeights[pos.index].boneIndex3 , weight = boneWeights[pos.index].weight3},
                };

                    int[] boneidRemaped;
                    float[] weightRemaped;



                    if (renderers[pos.index] is SkinnedMeshRenderer)
                    {

                        var smr = (SkinnedMeshRenderer)renderers[pos.index];

                        var boneWeightOrgFilterd = boneWeightOrg.Where(x => 0 < x.weight);

                        var boneWeightRemaped = boneWeightOrgFilterd.Select(x =>
                        {
                            var remapedBone = pmx.BoneArray.Select((b, i) => new { id = i, bone = b }).Where(b => smr.bones[x.id].name.Equals(b.bone.BoneNameE)).FirstOrDefault();

                            return new { id = remapedBone == null ? 0 : remapedBone.id, weight = x.weight };
                        });


                        boneidRemaped = boneWeightRemaped.Select(x => x.id).Concat(Enumerable.Range(0, 4).Select(y => 0)).Take(4).ToArray();
                        weightRemaped = boneWeightRemaped.Select(x => x.weight).Concat(Enumerable.Range(0, 4).Select(y => 0.0f)).Take(4).ToArray();
                    }
                    else
                    {
                        //var parentBone = pmx.BoneArray.Select((b, i) => new { id = i, bone = b }).Where(pb => pb.bone.BoneNameE.Equals(renderers[pos.index].transform.parent.name)).FirstOrDefault();
                        var parentBone = pmx.BoneArray.Select((b, i) => new { id = i, bone = b }).Where(pb => pb.bone.BoneNameE.Equals(renderers[pos.index].transform.name)).FirstOrDefault();

                        boneidRemaped = new int[] { parentBone == null ? 0 : parentBone.id, 0, 0, 0 };
                        weightRemaped = new float[] { 1, 0, 0, 0 };
                    }

                    if (boneidRemaped.Length != 4)
                        Debug.LogError("boneidRemaped count " + boneidRemaped.Length);
                    if (weightRemaped.Length != 4)
                        Debug.LogError("weightRemaped count " + weightRemaped.Length);

                    buildVertices.Add(new PmxVertexData()
                    {
                        Pos = pos.vertex,
                        Normal = normals[pos.index],
                        Uv = uvs[pos.index],

                        WeightType = WeightType.BDEF4,
                        BoneId = boneidRemaped,
                        Weight = weightRemaped,

                        Edge = 1.0f,
                    });
                }

                pmx.VertexArray = buildVertices.ToArray();

                Debug.Log("VertexCount = " + pmx.VertexArray.Length);

                #endregion



                #region build_face

                pmx.VertexIndices = faces.SelectMany(x => x).ToArray();
                Debug.Log("FaceCount = " + pmx.VertexIndices.Length);

                #endregion


                #region build_material_wip

                //long faceBorder = 0;
                var textures = new List<string>();
                textures.AddRange(mattex.Select(x => x.Value).Distinct().ToArray());
                textures.AddRange(subtex.Distinct().ToArray());
                pmx.TextureFiles = textures.Where(x => 0 < x.Length).Distinct().ToArray();
                Debug.Log("TextureFiles Count = " + pmx.TextureFiles.Length);

                var orgMaterialTable = from face in faces.Select((f, i) => new { face = f, index = i })
                                       join tex in mattex.Select((t, i) => new { texture = t, index = i })
                                       on face.index equals tex.index
                                       join mat in materials.Select((m, i) => new { material = m, index = i })
                                       on face.index equals mat.index
                                       select new { face.face, tex.texture, mat.material };

                var buildMaterial = orgMaterialTable.Select(x =>
                {
                    var diffuse = new Vector4() { x = 1, y = 1, z = 1, w = 1 };
                    var ambient = new Vector4() { x = 0.5f, y = 0.5f, z = 0.5f, w = 0 };
                    var spec = new Vector4() { x = 0.5f, y = 0.5f, z = 0.5f, w = 0 };

                    var outline = new Vector4() { x = 0.0f, y = 0.0f, z = 0.0f, w = 1 };
                    var outlineWidth = 0.0f;

                    var bm = RenderMode.Opaque;
                    if (x.material.HasProperty("_BlendMode"))
                    {
                        bm = (RenderMode)x.material.GetFloat("_BlendMode");
                    }


                    var sphereTex = "";
                    if (x.material.HasProperty("_SphereAdd"))
                    {
                        var spheremat = x.material.GetTexture("_SphereAdd");
                        sphereTex = spheremat != null ? spheremat.name : "";
                    }


                    if (x.material.HasProperty("_OutlineWidth"))
                    {
                        outlineWidth = x.material.GetFloat("_OutlineWidth");
                    }
                    if (x.material.HasProperty("_OutlineColor"))
                    {
                        outline = x.material.GetVector("_OutlineColor");
                    }

                    RenderFlags renderFlags = 0;
                    if (x.material.HasProperty("_CullMode"))
                    {
                        int cullmode = x.material.GetInt("_CullMode");
                        if (cullmode == 0)
                            renderFlags |= RenderFlags.DOUBLE_SIDED;
                    }

                    var owm = OutlineWidthMode.None;
                    if (x.material.HasProperty("_OutlineWidthMode"))
                    {
                        owm = (OutlineWidthMode)x.material.GetFloat("_OutlineWidthMode");
                    }
                    if (owm != OutlineWidthMode.None)
                        renderFlags |= RenderFlags.EDGE;

                    try
                    {
                        var color = x.material.GetColor("_Color");
                        diffuse.x = color.r;
                        diffuse.y = color.g;
                        diffuse.z = color.b;
                        diffuse.w = color.a;
                    }
                    catch (System.Exception e) { }

                    try
                    {
                        var emissionColor = x.material.GetColor("_EmissionColor");
                        ambient.x = emissionColor.r;
                        ambient.y = emissionColor.g;
                        ambient.z = emissionColor.b;
                        //ambient.w = emissionColor.a;
                    }
                    catch (System.Exception e) { }

                    try
                    {
                        var shadowColor = x.material.GetColor("_ShadeColor");
                        spec.x = shadowColor.r;
                        spec.y = shadowColor.g;
                        spec.z = shadowColor.b;
                        //spec.w = shadowColor.a;
                    }
                    catch (System.Exception e) { }


                    return new PmxMaterialData()
                    {
                        FaceCount = x.face.Length,
                        TextureId = ArrayUtility.IndexOf(pmx.TextureFiles, x.texture.Value),
                        ToonId = -1,
                        SphereId = ArrayUtility.IndexOf(pmx.TextureFiles, sphereTex),
                        Mode = SphereMode.DISBLE,
                        MaterialName = x.material.renderQueue + "_" + x.texture.Key,
                        MaterialNameE = x.texture.Key,
                        Diffuse = diffuse,
                        Ambient = spec, //ambient,
                        Specular = ambient, //spec,
                        Shininess = 0.0f,
                        Flag = renderFlags | RenderFlags.GROUND_SHADOW | RenderFlags.SLEF_SHADOW | RenderFlags.TO_SHADOW_MAP,
                        Edge = outline,
                        EdgeThick = outlineWidth,                    
                        Script = bm.ToString(),
                    };
                });

                pmx.MaterialArray = buildMaterial.ToArray();
                Debug.Log("Material Count = " + pmx.MaterialArray.Length);

                pmx.TextureFiles = pmx.TextureFiles.Select(x => "tex\\" + x + ".png").ToArray();

                #endregion

                #region build_morph

                /*
                var rebuildMorph = morphs.Select(x =>
                    new PmxMorphData
                    {
                        MorphName = x.MorphName,
                        MorphNameE = x.MorphNameE,
                        MorphType = x.MorphType,
                        MorphArray = x.MorphArray.Where(delta => pmx.VertexArray[((PmxMorphVertexData)delta).Index].Pos != ((PmxMorphVertexData)delta).Position).ToArray()
                    });
                */

                foreach(var morphData in morphs.Where(x => x.MorphType == MorphType.VERTEX))
                {
                    //無移動の頂点モーフを除外
                    morphData.MorphArray = morphData.MorphArray
                        .Where(delta => pmx.VertexArray[((PmxMorphVertexData)delta).Index].Pos != ((PmxMorphVertexData)delta).Position).ToArray();
                }

                pmx.MorphArray = morphs.ToArray();

                #endregion


                #endregion


            }


            slots.Add(new PmxSlotData()
            {
                Indices = new int[]{ 0 },
                NormalSlot = false,
                Type = SlotType.BONE,
                SlotName = "Root",
                SlotNameE = "Root"
            });
            slots.Add(new PmxSlotData()
            {
                Indices = pmx.MorphArray.Select((x, i) => i).ToArray(),
                NormalSlot = false,
                Type = SlotType.MORPH,
                SlotName = "表情",
                SlotNameE = "Exp"
            });
            slots.Add(new PmxSlotData()
            {
                Indices = pmx.BoneArray.Select((x, i) => i).Where((x)=> x != 0).ToArray(),
                NormalSlot = true,
                Type = SlotType.BONE,
                SlotName = "ボーン",
                SlotNameE = "Bones"
            });


            #region Rigid          //Rigid
            {

                //jointは、先端側に付ける　ばねに200、他全部０
                List<PmxRigidData> rigids = new List<PmxRigidData>();
                List<PmxJointData> joints = new List<PmxJointData>();


                System.Func< PmxBoneData[], string ,int> getIdFrom = 
                    (PmxBoneData[] array, string name) => 
                    array.Where(x => x.BoneNameE.Equals(name)).Select(x => x.BoneId).DefaultIfEmpty(-1).FirstOrDefault();

                rigids.Add(new PmxRigidData()
                {
                    RigidName = "AntiGravity",
                    RigidNameE = "AntiGravity",
                    BoneId = getIdFrom(pmx.BoneArray, "secondary"),
                    Pos = new Vector3(0, 0, 0),
                    RigidType = RigidType.Static,
                    Shape = RigidShape.Sphere,
                    Size = new Vector3(0, 0, 0),
                    Mass = 1.0f,
                    MovingAttenuation = 1.0f,
                    RotationAttenuation = 1.0f,
                    Repulsive = 0.0f,
                    Frictional = 0.5f,
                    GroupFlag = 0,
                    Group = 15,
                });


                //================
                //coliderも追加する　面倒なのでグループは全部１
                var springBoneColiders = GetComponentsInChildren<VRM.VRMSpringBoneColliderGroup>();
                foreach(var coliderGroup in springBoneColiders)
                {
                    var root = coliderGroup.transform;
                    int count = 0;

                    foreach(var colider in coliderGroup.Colliders)
                    {
                        var radius = colider.Radius * caScaling;

                        var pos = root.position + root.TransformVector(colider.Offset);

                        var name = root.name + "_" + (count++);

                        Debug.Log("ColiderGroup root= " + root.name + " ID:" + getIdFrom(pmx.BoneArray, root.name));

                        rigids.Add(new PmxRigidData()
                        {
                            RigidName = name,
                            RigidNameE = name,
                            BoneId = getIdFrom(pmx.BoneArray,root.name),
                            Pos = new Vector3(pos.x, pos.y, pos.z),
                            RigidType = RigidType.Static,
                            Shape = RigidShape.Sphere,
                            Size = new Vector3(radius, radius, radius),
                            Mass = 1.0f,
                            MovingAttenuation = 1.0f,
                            RotationAttenuation = 1.0f,
                            Repulsive = 0.0f,
                            Frictional = 0.5f,
                            GroupFlag = 0xFFFF,
                            Group = 2,
                        });
                    }
                }

                //================

                var springBones = GetComponentsInChildren<VRM.VRMSpringBone>().Distinct();
                foreach(var sBone in springBones)
                {
                    //柔らかさ
                    float fStiffness = sBone.m_stiffnessForce;

                    //空気抵抗的な
                    float fDrag = sBone.m_dragForce;

                    //球サイズ PMXRigidSphereでは10倍くらい
                    float radius = sBone.m_hitRadius * caScaling;

                    ushort colider = (ushort)(0 < sBone.ColliderGroups.Count() ? 0xFFFE: 0);


                    foreach (var root in sBone.RootBones) {

                        if (rigids.Exists(x => x.RigidNameE.Equals(root.name)))
                            continue;

                        Transform parent = root.parent;

                        if(!rigids.Exists(x => x.RigidNameE.Equals(parent.name)))
                        {
                            rigids.Add(new PmxRigidData()
                            {
                                RigidName = parent.name,
                                RigidNameE = parent.name,
                                BoneId = getIdFrom(pmx.BoneArray, parent.name),
                                Pos = new Vector3(parent.position.x, parent.position.y, parent.position.z),
                                RigidType = RigidType.Static,
                                Shape = RigidShape.Sphere,
                                Size = new Vector3(radius, radius, radius),
                                Mass = 1.0f,
                                MovingAttenuation = 1.0f,
                                RotationAttenuation = 1.0f,
                                Repulsive = 0.0f,
                                Frictional = 0.5f,
                            });
                        }
                        int rootRigidId = rigids.Select((x, i) => new { rigid = x, i })
                            .Where(y => y.rigid.RigidName.Equals(parent.name))
                            .Select(x => x.i).FirstOrDefault();

                        float SpringBaseFactor = 5;//30 = mass 1.0
                        float SpringReverseFactor = 0.5f;
                        float MassBase = 0.05f;
                        float MassDecFactor = 0.3f;

                        float MAbase = 1.0f;
                        float MADecFactor = 0.90f;

                        float RAFactor = 0.9999f;
                        float RAIncFactor = 0.80f;
                        
                        bool useRootForce = false;
                        float SpringRootFactor = 1.0f;

                        Stack<Transform> workTransforms = new Stack<Transform>();
                        workTransforms.Push(root);
                        do
                        {
                            var tmp = workTransforms.Pop();

                            rigids.Add(new PmxRigidData()
                            {
                                RigidName = tmp.name,
                                RigidNameE = tmp.name,
                                BoneId = getIdFrom(pmx.BoneArray, tmp.name),
                                Pos = new Vector3(tmp.position.x, tmp.position.y, tmp.position.z),
                                RigidType = RigidType.Hybrid,
                                Shape = RigidShape.Sphere,
                                Size = new Vector3(radius, radius, radius),
                                Mass = MassBase,
                                MovingAttenuation = MAbase,
                                RotationAttenuation = 1.0f - (1.0f - RAFactor) * fDrag,
                                Repulsive = 0.0f,
                                Frictional = 0.5f,
                                GroupFlag = (ushort)(tmp==root ? 0 : colider),
                            });
                            int currentRigidId = rigids.Count - 1;

                            {
                                int aId = rigids.Select((x, i) => new { rigid = x, i })
                                    .Where(y => y.rigid.RigidName.Equals(tmp.parent.name))
                                    .Select(x => x.i).FirstOrDefault();
                                int bId = currentRigidId;

                                joints.Add(new PmxJointData()
                                {
                                    JointName = tmp.name,
                                    JointNameE = tmp.name,
                                    RigidBodyA = aId,
                                    RigidBodyB = bId,
                                    //rootForceがある場合いらないかも
                                    SpringConstantRot = new Vector3(SpringBaseFactor, SpringBaseFactor, SpringBaseFactor) * fStiffness,

                                    JointType = JointType.Generic6DofSpring,

                                    Pos = new Vector3(tmp.position.x, tmp.position.y, tmp.position.z),

                                    //何故か初期向きを傾けると縦揺れnoiseが発生しない
                                    Rot = new Vector3(Mathf.Deg2Rad * 90, 0, Mathf.Deg2Rad * 90),

                                    //Min>Maxだとフリー
                                    //PosMin = new Vector3(1, 1, 1),
                                    //PosMax = new Vector3(0, 0, 0),

                                    //Min>Maxだとフリー
                                    //RotMin = new Vector3(1, 1, 1),
                                    //RotMax = new Vector3(0, 0, 0),
                                    RotMin = new Vector3(1, 1, 1) * Mathf.Deg2Rad * (fStiffness < 1.0f ? -90 : 0) * (fDrag < 0.1f ? 0 : fDrag),
                                    RotMax = new Vector3(1, 1, 1) * Mathf.Deg2Rad * (fStiffness < 1.0f ? 90 : 0) * (fDrag < 0.1f ? 0 : fDrag),
                                });

                            }
                            //固くしたい戻る力が大きいときは逆Jointはつけない
                            if (useReverseJoint && fStiffness < 1.0f)
                            {
                                int aId = rigids.Select((x, i) => new { rigid = x, i })
                                    .Where(y => y.rigid.RigidName.Equals(tmp.parent.name))
                                    .Select(x => x.i).FirstOrDefault();
                                int bId = currentRigidId;

                                joints.Add(new PmxJointData()
                                {
                                    JointName = tmp.name + "_r",
                                    JointNameE = tmp.name + "_r",
                                    RigidBodyA = bId,
                                    RigidBodyB = aId,

                                    //rootForceがある場合いらないかも
                                    SpringConstantRot = new Vector3(SpringBaseFactor, SpringBaseFactor, SpringBaseFactor) * fStiffness * SpringReverseFactor,

                                    JointType = JointType.Generic6DofSpring,
                                    Pos = new Vector3(tmp.position.x, tmp.position.y, tmp.position.z),

                                    //何故か初期向きを傾けると縦揺れnoiseが発生しない
                                    Rot = new Vector3(Mathf.Deg2Rad * 90, 0, Mathf.Deg2Rad * 90),

                                    //Min>Maxだとフリー
                                    //PosMin = new Vector3(1, 1, 1),
                                    //PosMax = new Vector3(0, 0, 0),

                                    //Min>Maxだとフリー
                                    RotMin = new Vector3(1, 1, 1),
                                    RotMax = new Vector3(0, 0, 0),
                                });

                            }

                            if (useAntiGravity)
                            {
                                int aId = rootRigidId;
                                int bId = currentRigidId;
                                joints.Add(new PmxJointData()
                                {
                                    JointName = tmp.name + "_ag",
                                    JointNameE = tmp.name + "_ag",
                                    RigidBodyA = 0,
                                    RigidBodyB = bId,
                                    //SpringConstantRot = new Vector3(SpringRootFactor, SpringRootFactor, SpringRootFactor) * fStiffness,
                                    SpringConstantPos = new Vector3(0, 9.81f, 0) * MassBase,
                                    JointType = JointType.Generic6DofSpring,

                                    //Min>Maxだとフリー
                                    PosMin = new Vector3(1, 1, 1),
                                    PosMax = new Vector3(0, 0, 0),

                                    //Min>Maxだとフリー
                                    RotMin = new Vector3(1, 1, 1),
                                    RotMax = new Vector3(0, 0, 0),

                                    //ジョイントから剛体が遠いほど回転のばね効果が減る
                                    //近
                                    Pos = new Vector3(tmp.position.x, tmp.position.y, tmp.position.z),
                                    //遠
                                    //Pos = new Vector3(parent.position.x, parent.position.y, parent.position.z),

                                    //何故か初期向きを傾けると縦揺れnoiseが発生しない
                                    //Rot = new Vector3(Mathf.Deg2Rad * 90, 0, Mathf.Deg2Rad * 90),
                                });
                            }


                            if (useRootForce){//回転だけ親だけでなくルートにも縛る
                                int aId = rootRigidId;
                                int bId = currentRigidId;
                                joints.Add(new PmxJointData()
                                {
                                    JointName = tmp.name + "_rf",
                                    JointNameE = tmp.name + "_rf",
                                    RigidBodyA = aId,
                                    RigidBodyB = bId,
                                    SpringConstantRot = new Vector3(SpringRootFactor, SpringRootFactor, SpringRootFactor) * fStiffness,
                                    JointType = JointType.Generic6DofSpring,

                                    //Min>Maxだとフリー
                                    PosMin = new Vector3(1, 1, 1),
                                    PosMax = new Vector3(0, 0, 0),

                                    //Min>Maxだとフリー
                                    RotMin = new Vector3(1, 1, 1),
                                    RotMax = new Vector3(0, 0, 0),
                                    
                                    //ジョイントから剛体が遠いほど回転のばね効果が減る
                                    //近
                                    Pos = new Vector3(tmp.position.x, tmp.position.y, tmp.position.z),
                                    //遠
                                    //Pos = new Vector3(parent.position.x, parent.position.y, parent.position.z),

                                    //何故か初期向きを傾けると縦揺れnoiseが発生しない
                                    Rot = new Vector3(Mathf.Deg2Rad * 90, 0, Mathf.Deg2Rad * 90),
                                });
                            }





                            for (int idx = 0; idx < tmp.childCount; idx++)
                                workTransforms.Push(tmp.GetChild(idx));
                            if (tmp.childCount == 0)
                            {
                                //pos=親位置から現位置の方角をノーマライズしての7％の先にずらした位置
                                Vector3 comPos = tmp.position - tmp.parent.position;
                                if(1.0 < Vector3.Magnitude(comPos) || emulateSpringBoneLeafColider)
                                    comPos = Vector3.Normalize(comPos);
                                comPos *= caScaling;
                                comPos *= 0.07f;
                                comPos += tmp.position;

                                rigids.Add(new PmxRigidData()
                                {
                                    RigidName = tmp.name + "_com",
                                    RigidNameE = tmp.name + "_com",
                                    BoneId = -1,
                                    Pos = new Vector3(comPos.x, comPos.y, comPos.z),
                                    RigidType = RigidType.Dynamic,
                                    Shape = RigidShape.Sphere,
                                    Size = new Vector3(radius, radius, radius),
                                    Mass = MassBase,

                                    MovingAttenuation = MAbase,
                                    //MovingAttenuation = 0.9f,
                                    RotationAttenuation = 1.0f - (1.0f - RAFactor) * fDrag,

                                    Repulsive = 0.0f,
                                    Frictional = 0.5f,
                                    GroupFlag = colider,
                                });

                                {
                                    int aId = currentRigidId;
                                    int bId = rigids.Count - 1; //comId

                                    joints.Add(new PmxJointData()
                                    {
                                        JointName = tmp.name + "_com",
                                        JointNameE = tmp.name + "_com",
                                        RigidBodyA = aId,
                                        RigidBodyB = bId,
                                        SpringConstantRot = new Vector3(0, 0, 0),
                                        JointType = JointType.Generic6DofSpring,
                                        Pos = new Vector3(comPos.x, comPos.y, comPos.z),
                                        Rot = new Vector3(Mathf.Deg2Rad * 90, 0, Mathf.Deg2Rad * 90),
                                    });

                                    if (fStiffness < 1.0f)
                                    {
                                        joints.Add(new PmxJointData()
                                        {
                                            JointName = tmp.name + "_com_r",
                                            JointNameE = tmp.name + "_com_r",
                                            RigidBodyA = bId,
                                            RigidBodyB = aId,
                                            SpringConstantRot = new Vector3(0, 0, 0),
                                            JointType = JointType.Generic6DofSpring,
                                            Pos = new Vector3(comPos.x, comPos.y, comPos.z),
                                            Rot = new Vector3(Mathf.Deg2Rad * 90, 0, Mathf.Deg2Rad * 90),
                                        });
                                    }

                                    if (useAntiGravity)
                                    {
                                        joints.Add(new PmxJointData()
                                        {
                                            JointName = tmp.name + "_com_ag",
                                            JointNameE = tmp.name + "_com_ag",
                                            RigidBodyA = 0,
                                            RigidBodyB = bId,
                                            //SpringConstantRot = new Vector3(SpringRootFactor, SpringRootFactor, SpringRootFactor) * fStiffness,
                                            SpringConstantPos = new Vector3(0, 9.81f, 0) * MassBase,
                                            JointType = JointType.Generic6DofSpring,

                                            //Min>Maxだとフリー
                                            PosMin = new Vector3(1, 1, 1),
                                            PosMax = new Vector3(0, 0, 0),

                                            //Min>Maxだとフリー
                                            RotMin = new Vector3(1, 1, 1),
                                            RotMax = new Vector3(0, 0, 0),

                                            //ジョイントから剛体が遠いほど回転のばね効果が減る
                                            //近
                                            Pos = new Vector3(tmp.position.x, tmp.position.y, tmp.position.z),
                                            //遠
                                            //Pos = new Vector3(parent.position.x, parent.position.y, parent.position.z),

                                            //何故か初期向きを傾けると縦揺れnoiseが発生しない
                                            //Rot = new Vector3(Mathf.Deg2Rad * 90, 0, Mathf.Deg2Rad * 90),
                                        });
                                    }
                                }
                            }


                            //next calc

                            MassBase *= MassDecFactor;
                            MAbase *= MADecFactor;
                            MAbase = Mathf.Max(0.5f, MAbase);

                            RAFactor *= RAIncFactor;

                        } while (workTransforms.Count != 0);
                    }
                }

                pmx.RigidArray = rigids.ToArray();
                pmx.JointArray = joints.ToArray();
            }

            #endregion






            pmx.SlotArray = slots.ToArray();

            {
                //pmx.Header.Version = 2.0f;
                pmx.Header.NumberOfExtraUv = 0;
                pmx.Header.VertexIndexSize = PmxHeaderData.CalcIndexSize(pmx.VertexArray.Length);
                pmx.Header.TextureIndexSize = PmxHeaderData.CalcIndexSize(pmx.TextureFiles.Length);
                pmx.Header.MaterialIndexSize = PmxHeaderData.CalcIndexSize(pmx.MaterialArray.Length);
                pmx.Header.BoneIndexSize = PmxHeaderData.CalcIndexSize(pmx.BoneArray.Length);
                pmx.Header.MorphIndexSize = PmxHeaderData.CalcIndexSize(pmx.MorphArray.Length);
                pmx.Header.RigidIndexSize = PmxHeaderData.CalcIndexSize(pmx.RigidArray.Length);

                pmx.Header.ModelName = name;
                pmx.Header.ModelNameE = name;
                pmx.Header.Description = "Unity PMXExporter exported.";
                pmx.Header.DescriptionE = "Unity PMXExporter exported.";
            };

            var filename = savepath;
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                pmx.Write(bw);
            }

            Debug.Log(filename + " is exported.");
        }
        finally
        {
            if (ConvertArmatuar)
            {
                if (tmpRootRot != null) {
                    transform.localScale /= caScaling;
                    transform.rotation = (Quaternion)tmpRootRot;
                }
                var anim = GetComponent<Animator>();
                var lua = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                if(tmpLUARot != null)
                    lua.rotation = (Quaternion)tmpLUARot;

                var rua = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
                if (tmpRUARot != null)
                    rua.rotation = (Quaternion)tmpRUARot;
            }
        }

    }
#endif
}