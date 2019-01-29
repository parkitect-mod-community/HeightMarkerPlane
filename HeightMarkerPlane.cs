/**
* Copyright 2019 Michael Pollind
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/


using System.Collections.Generic;
using UnityEngine;

namespace TrackHeightPlane
{
    public class HeightMarkerPlane : MonoBehaviour
    {
        public TrackSegment4 TrackSegment { get; private set; }
        private float _update = 0.0f;
        
        public static Material _MaterialPlane = null;
        private GameObject _heightMarkerGo;
        private MeshFilter _meshFilter;
        public static Material GetMaterialPlane()
        {
            if (_MaterialPlane == null)
            {
                _MaterialPlane = new Material(Shader.Find("GUI/Text Shader"));
                _MaterialPlane.SetColor("_TintColor", new Color(255, 255, 255, 100));
                _MaterialPlane.SetTexture("_MainTex",
                    AssetManager.Instance.terrainGridProjectorGO.GetComponent<Light>().cookie);
                _MaterialPlane.SetTextureScale("_MainTex", new Vector2(1.0f, 1.0f));
                _MaterialPlane.SetTextureOffset("_MainTex", new Vector2(0f, .5f));
            }

            return _MaterialPlane;
        }

        private void Awake()
        {
            TrackSegment = GetComponent<TrackSegment4>();
            
            _heightMarkerGo = new GameObject("HeightMarkerPlane");
            _meshFilter = _heightMarkerGo.AddComponent<MeshFilter>();
            var meshRenderer = _heightMarkerGo.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = GetMaterialPlane();
//            _heightMarkerGo.transform.SetParent(transform);

        }

        private void OnDestroy()
        {
            Destroy(_heightMarkerGo);
        }

        private void Update()
        {
            if (TrackSegment != null)
            {
                transform.parent = TrackSegment.transform;
                
                if (Time.time - _update > .5f)
                {

                    _heightMarkerGo.transform.position = Vector3.zero;
                    _heightMarkerGo.transform.rotation = Quaternion.identity;
                    
                    var verticies = new List<Vector3>();
                    var triangles = new List<int>();
                    var uvs = new List<Vector2>();

                    var sample = TrackSegment.getLength() / Mathf.RoundToInt(TrackSegment.getLength() / .2f);
                    var pos = 0.0f;

                    var tForDistance = TrackSegment.getTForDistance(0);
                    var position = TrackSegment.getPoint(tForDistance);

                    var terrain = GameController.Instance.park.getTerrain(transform.position);
                    var vector = position;
                    if (terrain != null) vector = terrain.getPoint(transform.position);
                    var magnitude = (position - vector).magnitude;


                    verticies.Add(position);
                    verticies.Add(
                        position + Vector3.down * magnitude *
                                                        Mathf.Sign(position.y - vector.y));

                    uvs.Add(new Vector2(0, magnitude));
                    uvs.Add(new Vector2(0, 0));

                    var previous = position;
                    float xoffset = 0;
                    while (pos < TrackSegment.getLength())
                    {
                        tForDistance = TrackSegment.getTForDistance(pos);
                        pos += sample;

                        position = TrackSegment.getPoint(tForDistance);

                        terrain = GameController.Instance.park.getTerrain(position);
                        vector = position;
                        if (terrain != null) vector = terrain.getPoint(position);
                        magnitude = (position - vector).magnitude;


                        verticies.Add(position);
                        verticies.Add(position + Vector3.down * magnitude * Mathf.Sign(position.y - vector.y));

                        xoffset += Vector3.Distance(previous, position);
                        uvs.Add(new Vector2(xoffset, vector.y + magnitude));
                        uvs.Add(new Vector2(xoffset, vector.y - 0));


                        var last = verticies.Count - 1;
                        triangles.Add(last - 3);
                        triangles.Add(last - 2);
                        triangles.Add(last - 1);

                        triangles.Add(last - 1);
                        triangles.Add(last - 2);
                        triangles.Add(last);

                        previous = position;
                    }

                    _meshFilter.mesh.vertices = verticies.ToArray();
                    _meshFilter.mesh.triangles = triangles.ToArray();
                    _meshFilter.mesh.uv = uvs.ToArray();
                    _update = Time.time;
                }
            }
        }

    }
}