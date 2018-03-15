﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KerbalKonstructs.Utilities
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    class DebugDrawer : MonoBehaviour
    {
        private static readonly List<Line> lines = new List<Line>();
        private static readonly List<Point> points = new List<Point>();
        private static readonly List<Trans> transforms = new List<Trans>();
        public Material lineMaterial;

        private struct Line
        {
            public readonly Vector3 start;
            public readonly Vector3 end;
            public readonly Color color;

            public Line(Vector3 start, Vector3 end, Color color)
            {
                this.start = start;
                this.end = end;
                this.color = color;
            }
        }

        private struct Point
        {
            public readonly Vector3 pos;
            public readonly Color color;

            public Point(Vector3 pos, Color color)
            {
                this.pos = pos;
                this.color = color;
            }
        }

        private struct Trans
        {
            public readonly Vector3 pos;
            public readonly Vector3 up;
            public readonly Vector3 right;
            public readonly Vector3 forward;

            public Trans(Vector3 pos, Vector3 up, Vector3 right, Vector3 forward)
            {
                this.pos = pos;
                this.up = up;
                this.right = right;
                this.forward = forward;
            }
        }

        public static void DebugLine(Vector3 start, Vector3 end, Color col)
        {
            lines.Add(new Line(start, end, col));
        }

        /// <summary>
        /// Paints the vector from the start point
        /// </summary>
        /// <param name="start"></param>
        /// <param name="vector"></param>
        /// <param name="col"></param>
        public static void DebugVector(Vector3 start, Vector3 vector, Color col)
        {
            lines.Add(new Line(start, start + vector, col));
        }

        public static void DebugPoint(Vector3 start, Color col)
        {
            points.Add(new Point(start, col));
        }

        public static void DebugTransforms(Transform t)
        {
            transforms.Add(new Trans(t.position, t.up, t.right, t.forward));
        }

        private void Start()
        {
            DontDestroyOnLoad(this);
            if (!lineMaterial)
            {
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                lineMaterial.SetInt("_ZWrite", 0);
                lineMaterial.SetInt("_ZWrite", (int)UnityEngine.Rendering.CompareFunction.Always);
            }
            StartCoroutine("EndOfFrameDrawing");
        }

        private IEnumerator EndOfFrameDrawing()
        {
            Debug.Log("DebugDrawer starting");
            while (true)
            {
                if ((lines.Count + points.Count + transforms.Count) == 0 )
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }
                yield return new WaitForEndOfFrame();
                try
                {
                    transform.position = Vector3.zero;

                    Camera cam = GetActiveCam();
                    GL.PushMatrix();
                    lineMaterial.SetPass(0);

                    // In a modern Unity we would use cam.projectionMatrix.decomposeProjection to get the decomposed matrix
                    // and Matrix4x4.Frustum(FrustumPlanes frustumPlanes) to get a new one

                    // Change the far clip plane of the projection matrix
                    Matrix4x4 projectionMatrix = Matrix4x4.Perspective(cam.fieldOfView, cam.aspect, cam.nearClipPlane, float.MaxValue);
                    GL.LoadProjectionMatrix(projectionMatrix);
                    GL.MultMatrix(cam.worldToCameraMatrix);
                    //GL.Viewport(new Rect(0, 0, Screen.width, Screen.height));

                    GL.Begin(GL.LINES);

                    for (int i = 0; i < lines.Count; i++)
                    {
                        Line line = lines[i];
                        DrawLine(line.start, line.end, line.color);
                    }

                    for (int i = 0; i < points.Count; i++)
                    {
                        Point point = points[i];
                        DrawPoint(point.pos, point.color);
                    }

                    for (int i = 0; i < transforms.Count; i++)
                    {
                        Trans t = transforms[i];
                        DrawTransform(t.pos, t.up, t.right, t.forward);
                    }

                    GL.End();
                    GL.PopMatrix();

                    lines.Clear();
                    points.Clear();
                    transforms.Clear();
                }
                catch (Exception) { }
            }
        }

        private static Camera GetActiveCam()
        {
            Camera cam;

            if (!HighLogic.fetch)
                return Camera.main;

            if (HighLogic.LoadedSceneIsEditor)
                cam = EditorLogic.fetch.editorCamera;
            else if (HighLogic.LoadedSceneIsFlight)
                cam = MapView.MapIsEnabled ? PlanetariumCamera.Camera : FlightCamera.fetch.mainCamera;
            else
                cam = Camera.main;
            return cam;
        }

        private static void DrawLine(Vector3 origin, Vector3 destination, Color color)
        {
            GL.Color(color);
            GL.Vertex(origin);
            GL.Vertex(destination);
        }

        private static void DrawRay(Vector3 origin, Vector3 direction, Color color)
        {
            GL.Color(color);
            GL.Vertex(origin);
            GL.Vertex(origin + direction);
        }

        private static void DrawTransform(Vector3 position, Vector3 up, Vector3 right, Vector3 forward, float scale = 1.0f)
        {
            DrawRay(position, up * scale, Color.green);
            DrawRay(position, right * scale, Color.red);
            DrawRay(position, forward * scale, Color.blue);
        }

        private static void DrawPoint(Vector3 position, Color color, float scale = 1.0f)
        {
            DrawRay(position + Vector3.up * (scale * 0.5f), -Vector3.up * scale, color);
            DrawRay(position + Vector3.right * (scale * 0.5f), -Vector3.right * scale, color);
            DrawRay(position + Vector3.forward * (scale * 0.5f), -Vector3.forward * scale, color);
        }
    }
}
