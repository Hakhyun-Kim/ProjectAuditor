using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Unity.ProjectAuditor.Editor;
using Unity.ProjectAuditor.Editor.Auditors;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_2018_2_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

namespace UnityEditor.ProjectAuditor.EditorTests
{
    class ShaderTests
    {
        TempAsset m_ShaderResource;
        TempAsset m_EditorShaderResource;

        TempAsset m_ShaderUsingBuiltInKeywordResource;
        TempAsset m_SurfShaderResource;

#if UNITY_2018_2_OR_NEWER
        static string s_KeywordName = "DIRECTIONAL";

        class StripVariants : IPreprocessShaders
        {
            public static bool Enabled;
            public int callbackOrder { get { return 0; } }

            public void OnProcessShader(
                Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> shaderCompilerData)
            {
                if (!Enabled)
                    return;

                var keyword = new ShaderKeyword(s_KeywordName);

                for (int i = 0; i < shaderCompilerData.Count; ++i)
                {
                    if (shaderCompilerData[i].shaderKeywordSet.IsEnabled(keyword))
                    {
                        shaderCompilerData.RemoveAt(i);
                        --i;
                    }
                }
            }
        }
#endif


        [OneTimeSetUp]
        public void SetUp()
        {
            m_ShaderResource = new TempAsset("Resources/MyTestShader.shader", @"
            Shader ""Custom/MyTestShader""
            {
                SubShader
                {
                    Pass
                    {
                        Name ""MyTestShader/Pass""

                        CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    #pragma multi_compile KEYWORD_A KEYWORD_B

                        struct appdata
                        {
                            float4 vertex : POSITION;
                            float2 uv : TEXCOORD0;
                        };

                        struct v2f
                        {
                            float2 uv : TEXCOORD0;
                            float4 vertex : SV_POSITION;
                        };

                        sampler2D _MainTex;
                        float4 _MainTex_ST;

                        v2f vert (appdata v)
                        {
                            v2f o;
                            o.vertex = UnityObjectToClipPos(v.vertex);
                            o.uv = v.uv;
                            return o;
                        }

                        fixed4 frag (v2f i) : SV_Target
                        {
                            return tex2D(_MainTex, i.uv);
                        }
                        ENDCG
                    }
                }
            }");


            m_ShaderUsingBuiltInKeywordResource = new TempAsset("Resources/ShaderUsingBuiltInKeyword.shader", @"
Shader ""Custom/ShaderUsingBuiltInKeyword""
            {
                Properties
                {
                    _MainTex (""Texture"", 2D) = ""white"" {}
                }
                SubShader
                {
                    Tags { ""RenderType""=""Opaque"" }
                    LOD 100

                    Pass
                    {
                        CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_instancing

#include ""UnityCG.cginc""

                        struct appdata
                        {
                            float4 vertex : POSITION;
                            float2 uv : TEXCOORD0;
                        };

                        struct v2f
                        {
                            float2 uv : TEXCOORD0;
                            float4 vertex : SV_POSITION;
                        };

                        sampler2D _MainTex;
                        float4 _MainTex_ST;

                        v2f vert (appdata v)
                        {
                            v2f o;
                            o.vertex = UnityObjectToClipPos(v.vertex);
                            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                            return o;
                        }

                        fixed4 frag (v2f i) : SV_Target
                        {
                            return tex2D(_MainTex, i.uv);
                        }
                        ENDCG
                    }
                }
            }
            ");

            m_SurfShaderResource = new TempAsset("Resources/MySurfShader.shader", @"
Shader ""Custom/MySurfShader""
            {
                Properties
                {
                    _Color (""Color"", Color) = (1,1,1,1)
                }
                SubShader
                {
                    Tags { ""RenderType""=""Opaque"" }
                    LOD 200

                    CGPROGRAM
                    // Physically based Standard lighting model, and enable shadows on all light types
                    #pragma surface surf Standard fullforwardshadows

                    // Use shader model 3.0 target, to get nicer looking lighting
                    #pragma target 3.0

                    sampler2D _MainTex;

                    struct Input
                    {
                        float2 uv_MainTex;
                    };

                    half _Glossiness;
                    half _Metallic;
                    fixed4 _Color;

                    void surf (Input IN, inout SurfaceOutputStandard o)
                    {
                        // Albedo comes from a texture tinted by color
                        fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
                        o.Albedo = c.rgb;
                        // Metallic and smoothness come from slider variables
                        o.Metallic = _Metallic;
                        o.Smoothness = _Glossiness;
                        o.Alpha = c.a;
                    }
                    ENDCG
                }
                FallBack ""Diffuse""
            }
");

            m_EditorShaderResource = new TempAsset("Editor/MyEditorShader.shader", @"
Shader ""Custom/MyEditorShader""
            {
                Properties
                {
                    _Color (""Color"", Color) = (1,1,1,1)
                }
                SubShader
                {
                    Tags { ""RenderType""=""Opaque"" }
                    LOD 200

                    CGPROGRAM
                    // Physically based Standard lighting model, and enable shadows on all light types
                    #pragma surface surf Standard fullforwardshadows

                    // Use shader model 3.0 target, to get nicer looking lighting
                    #pragma target 3.0

                    sampler2D _MainTex;

                    struct Input
                    {
                        float2 uv_MainTex;
                    };

                    half _Glossiness;
                    half _Metallic;
                    fixed4 _Color;

                    void surf (Input IN, inout SurfaceOutputStandard o)
                    {
                        // Albedo comes from a texture tinted by color
                        fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
                        o.Albedo = c.rgb;
                        // Metallic and smoothness come from slider variables
                        o.Metallic = _Metallic;
                        o.Smoothness = _Glossiness;
                        o.Alpha = c.a;
                    }
                    ENDCG
                }
                FallBack ""Diffuse""
            }
");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            TempAsset.Cleanup();
        }

#if UNITY_2018_2_OR_NEWER
        [Test]
        public void ShaderVariantsRequireBuild()
        {
            ShadersAuditor.CleanupBuildData();
            var projectAuditor = new Unity.ProjectAuditor.Editor.ProjectAuditor();

            var projectReport = projectAuditor.Audit();
            var issues = projectReport.GetIssues(IssueCategory.ShaderVariants);
            Assert.Positive(issues.Length);
            Assert.True(issues.First().description.Equals("Build the project to view the Shader Variants"));
        }

        [Test]
        public void ShaderVariantsAreReported()
        {
            var issues = BuildAndAnalyze();

            var keywords = issues.Select(i => i.GetCustomProperty((int)ShaderVariantProperty.Keywords));

            Assert.True(keywords.Any(key => key.Equals(s_KeywordName)));

            var variants = issues.Where(i => i.description.Equals("Custom/MyTestShader")).ToArray();
            Assert.Positive(variants.Count());

            // check custom property
            Assert.True(variants.Any(v => v.GetCustomProperty((int)ShaderVariantProperty.Keywords).Equals("KEYWORD_A")));
            Assert.True(variants.Any(v => v.GetCustomProperty((int)ShaderVariantProperty.Keywords).Equals("KEYWORD_B")));
            Assert.AreEqual((int)ShaderVariantProperty.Num, variants.First().GetNumCustomProperties());
        }

        [Test]
        public void ShaderVariantForBuiltInKeywordIsReported()
        {
            var issues = BuildAndAnalyze();

            var keywords = issues.Select(i => i.GetCustomProperty((int)ShaderVariantProperty.Keywords));

            Assert.True(keywords.Any(key => key.Equals(s_KeywordName)));

            var variants = issues.Where(i => i.description.Equals("Custom/ShaderUsingBuiltInKeyword")).ToArray();
            Assert.Positive(variants.Count(), "No shader variants found");

            foreach (var variant in variants)
            {
                Debug.Log("Project Auditor variant debugging: " + variant.GetCustomProperty((int)ShaderVariantProperty.Keywords));
                Debug.LogWarning("Project Auditor variant debugging: " + variant.GetCustomProperty((int)ShaderVariantProperty.Keywords));
                Console.WriteLine("Project Auditor variant debugging: " + variant.GetCustomProperty((int)ShaderVariantProperty.Keywords));
            }

            // check custom properties
            Assert.True(variants.Any(v => v.GetCustomProperty((int)ShaderVariantProperty.Keywords).Equals("<no keywords>")), "No shader variants found without INSTANCING_ON keyword");
            Assert.True(variants.Any(v => v.GetCustomProperty((int)ShaderVariantProperty.Requirements).Equals("BaseShaders, Derivatives")), "No shader variants found without Instancing requirement");
#if UNITY_2019_1_OR_NEWER
            Assert.True(variants.Any(v => v.GetCustomProperty((int)ShaderVariantProperty.Keywords).Equals("INSTANCING_ON")), "No shader variants found with INSTANCING_ON keyword");
            Assert.True(variants.Any(v => v.GetCustomProperty((int)ShaderVariantProperty.Requirements).Equals("BaseShaders, Derivatives, Instancing")), "No shader variants found with Instancing requirement");
#endif
        }

        [Test]
        public void SurfShaderVariantsAreReported()
        {
            var issues = BuildAndAnalyze();

            var keywords = issues.Select(i => i.GetCustomProperty((int)ShaderVariantProperty.Keywords));

            Assert.True(keywords.Any(key => key.Equals(s_KeywordName)));

            var variants = issues.Where(i => i.description.Equals("Custom/MySurfShader")).ToArray();
            Assert.Positive(variants.Count());

            // check custom property
            var variant = variants.FirstOrDefault(v => v.GetCustomProperty((int)ShaderVariantProperty.PassName).Equals("FORWARD") && v.GetCustomProperty((int)ShaderVariantProperty.Keywords).Equals("DIRECTIONAL"));
            Assert.NotNull(variant);
        }

        [Test]
        public void StrippedVariantsAreNotReported()
        {
            StripVariants.Enabled = true;
            var issues = BuildAndAnalyze();
            StripVariants.Enabled = false;

            var keywords = issues.Select(i => i.GetCustomProperty((int)ShaderVariantProperty.Keywords));

            Assert.False(keywords.Any(key => key.Equals(s_KeywordName)));
        }

        static ProjectIssue[] BuildAndAnalyze()
        {
            var buildPath = FileUtil.GetUniqueTempPathInProject();
            Directory.CreateDirectory(buildPath);
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new string[] {},
                locationPathName = Path.Combine(buildPath, "test"),
                target = EditorUserBuildSettings.activeBuildTarget,
                targetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget),
                options = BuildOptions.Development
            };
            var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

            Assert.True(buildReport.summary.result == BuildResult.Succeeded);

            Directory.Delete(buildPath, true);

            var projectAuditor = new Unity.ProjectAuditor.Editor.ProjectAuditor();
            var projectReport = projectAuditor.Audit();
            return projectReport.GetIssues(IssueCategory.ShaderVariants);
        }

#endif

        [Test]
        public void ShaderIsReported()
        {
            var projectAuditor = new Unity.ProjectAuditor.Editor.ProjectAuditor();
            var projectReport = projectAuditor.Audit();
            var issues = projectReport.GetIssues(IssueCategory.Shaders);
            var shaderIssue = issues.FirstOrDefault(i => i.description.Equals("Custom/MyTestShader"));
            Assert.NotNull(shaderIssue);

            // check custom property
            Assert.AreEqual((int)ShaderProperty.Num, shaderIssue.GetNumCustomProperties());
#if UNITY_2019_1_OR_NEWER
            Assert.AreEqual(1, shaderIssue.GetCustomPropertyAsInt((int)ShaderProperty.NumPasses), "NumPasses was : " + shaderIssue.GetCustomProperty((int)ShaderProperty.NumPasses));
            Assert.AreEqual(2, shaderIssue.GetCustomPropertyAsInt((int)ShaderProperty.NumKeywords), "NumKeywords was : " + shaderIssue.GetCustomProperty((int)ShaderProperty.NumKeywords));
#else
            Assert.AreEqual(0, shaderIssue.GetCustomPropertyAsInt((int)ShaderProperty.NumPasses), "NumPasses was : " + shaderIssue.GetCustomProperty((int)ShaderProperty.NumPasses));
            Assert.AreEqual(0, shaderIssue.GetCustomPropertyAsInt((int)ShaderProperty.NumKeywords), "NumKeywords was : " + shaderIssue.GetCustomProperty((int)ShaderProperty.NumKeywords));
#endif
            Assert.AreEqual(2000, shaderIssue.GetCustomPropertyAsInt((int)ShaderProperty.RenderQueue), "RenderQueue was : " + shaderIssue.GetCustomProperty((int)ShaderProperty.RenderQueue));
            Assert.True(shaderIssue.GetCustomProperty((int)ShaderProperty.Instancing).Equals("No"));
        }

        [Test]
        public void ShaderUsingBuiltInKeywordIsReported()
        {
            var projectAuditor = new Unity.ProjectAuditor.Editor.ProjectAuditor();
            var projectReport = projectAuditor.Audit();
            var issues = projectReport.GetIssues(IssueCategory.Shaders);
            var shaderIssue = issues.FirstOrDefault(i => i.description.Equals("Custom/ShaderUsingBuiltInKeyword"));
            Assert.NotNull(shaderIssue);

            // check custom property
            Assert.AreEqual((int)ShaderProperty.Num, shaderIssue.GetNumCustomProperties());
#if UNITY_2019_1_OR_NEWER
            Assert.AreEqual(1, shaderIssue.GetCustomPropertyAsInt((int)ShaderProperty.NumPasses), "NumPasses was : " + shaderIssue.GetCustomProperty((int)ShaderProperty.NumPasses));
            Assert.AreEqual(1, shaderIssue.GetCustomPropertyAsInt((int)ShaderProperty.NumKeywords), "NumKeywords was : " + shaderIssue.GetCustomProperty((int)ShaderProperty.NumKeywords));
#else
            Assert.AreEqual(0, shaderIssue.GetCustomPropertyAsInt((int)ShaderProperty.NumPasses), "NumPasses was : " + shaderIssue.GetCustomProperty((int)ShaderProperty.NumPasses));
            Assert.AreEqual(0, shaderIssue.GetCustomPropertyAsInt((int)ShaderProperty.NumKeywords), "NumKeywords was : " + shaderIssue.GetCustomProperty((int)ShaderProperty.NumKeywords));
#endif
            Assert.AreEqual(2000, shaderIssue.GetCustomPropertyAsInt((int)ShaderProperty.RenderQueue), "RenderQueue was : " + shaderIssue.GetCustomProperty((int)ShaderProperty.RenderQueue));
            Assert.True(shaderIssue.GetCustomProperty((int)ShaderProperty.Instancing).Equals("Yes"));
        }

        [Test]
        public void SurfShaderIsReported()
        {
            var projectAuditor = new Unity.ProjectAuditor.Editor.ProjectAuditor();
            var projectReport = projectAuditor.Audit();
            var issues = projectReport.GetIssues(IssueCategory.Shaders);
            var shaderIssue = issues.FirstOrDefault(i => i.description.Equals("Custom/MySurfShader"));
            Assert.NotNull(shaderIssue);

            // check custom property
            Assert.AreEqual((int)ShaderProperty.Num, shaderIssue.GetNumCustomProperties());
#if UNITY_2019_1_OR_NEWER
            Assert.AreEqual(4, shaderIssue.GetCustomPropertyAsInt((int)ShaderProperty.NumPasses), "NumPasses was : " + shaderIssue.GetCustomProperty((int)ShaderProperty.NumPasses));
            Assert.AreEqual(22, shaderIssue.GetCustomPropertyAsInt((int)ShaderProperty.NumKeywords), "NumKeywords was : " + shaderIssue.GetCustomProperty((int)ShaderProperty.NumKeywords));
#else
            Assert.AreEqual(0, shaderIssue.GetCustomPropertyAsInt((int)ShaderProperty.NumPasses), "NumPasses was : " + shaderIssue.GetCustomProperty((int)ShaderProperty.NumPasses));
            Assert.AreEqual(0, shaderIssue.GetCustomPropertyAsInt((int)ShaderProperty.NumKeywords), "NumKeywords was : " + shaderIssue.GetCustomProperty((int)ShaderProperty.NumKeywords));
#endif
            Assert.AreEqual(2000, shaderIssue.GetCustomPropertyAsInt((int)ShaderProperty.RenderQueue), "RenderQueue was : " + shaderIssue.GetCustomProperty((int)ShaderProperty.RenderQueue));
            Assert.True(shaderIssue.GetCustomProperty((int)ShaderProperty.Instancing).Equals("Yes"));
        }

        [Test]
        public void EditorShaderIsNotReported()
        {
            var projectAuditor = new Unity.ProjectAuditor.Editor.ProjectAuditor();

            var projectReport = projectAuditor.Audit();
            var issues = projectReport.GetIssues(IssueCategory.Shaders);
            issues = issues.Where(i => i.description.Equals("Custom/MyEditorShader")).ToArray();

            Assert.Zero(issues.Length);
        }
    }
}
