Shader "Unlit/XRayShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		//_XRayColor("XRay Color", COLOR) = (0,1,1,1)
		_XRayPower("XRay Power", Range(0.00001, 3)) = 0.001
	}
	SubShader
	{
		Tags { "Queue" = "Geometry+1000" "RenderType" = "Opaque" }
		LOD 100
		Pass{
			Stencil {
				Ref 254
				Comp Always
				Pass Replace
				ZFail Keep
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal:TEXCOORD1;
				float3 worldPos:TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _Steps;
			float _ToonLerpWeight;
			float4 _RimColor;
			float _RimPower;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld,v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// ���������� ����
				float3 ambient = UNITY_LIGHTMODEL_AMBIENT;
				// �������� ���� ��������
				fixed3 albedo = tex2D(_MainTex, i.uv).rgb;
				// ����������� �������
				float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				// ���������
				fixed3 worldLightDir = UnityWorldSpaceLightDir(i.worldPos);
				float halfLambert = dot(worldLightDir,i.worldNormal) * 0.5 + 0.5;
				// ��������� ��������� ���������
				fixed3 diffuse = _LightColor0.rgb * albedo * _Color.rgb * halfLambert;
				return fixed4(ambient + diffuse,1);
			}
				ENDCG
		}

			Pass{
				Tags{"ForceNoShadowCasting" = "true"}
				Name "XRay"
				// ��������� ������ ���������� �������� �������� �� ������� ���������� Queue
				Blend SrcAlpha one
				// ��������� �������� ������
				ZWrite off
				ZTest Greater // ����������, ����� ������ ��� (���� - �������� ������� �� ������)
				// ������������� ��������� ��� ����������. �� ������ ������� ������� �� ���������� �������� 254 � ����� ���������. � ������� Xray, ���� �������� ��������� ����� 254, ������ ������ �� ������������.
				Stencil {
					Ref 254
					Comp NotEqual
					Pass Keep
					ZFail Keep
				}

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				//float4 _XRayColor;
				float4 _Color;
				float _XRayPower;
			struct v2f {
				float4 vertex :SV_POSITION;
				// ����� 1, ��������� ������� �����, � ����� ���������� ����������� ������� �� ��������
				//float3 worldPos:TEXCOORD0;
				//float3 worldNormal:TEXCOORD1;
				// ����� ������, ���������� � ������������ ��������
				float3 viewDir:TEXCOORD0;
				float3 normal:TEXCOORD1;
			};

			v2f vert(appdata_base v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				// ����� ������, ��������� ����� ����, � ����� ���������� ����������� ������ �� ���������
				//o.worldNormal = UnityObjectToWorldNormal(v.normal);
				//o.worldPos = mul(unity_ObjectToWorld,v.vertex);
				// ����� ������, ��������� ����������� ������ � ������
				//o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = ObjSpaceViewDir(v.vertex);
				o.normal = v.normal;
				return o;
			}

			float4 frag(v2f i) :SV_Target{
				/// �������� ���� �� ����������� ������ � ����������� ������� ������� (����������� ������ � ����������� ������� ������� ��������������� ����)
				// ����� ������, ��������� ����� ����, � ����� ���������� ����������� ������ �� ���������
				//float3 worldNormal = normalize(i.worldNormal);
				//float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				// ����� ������, ��������� ����������� ������ � ������
				float3 worldNormal = normalize(i.normal);
				float3 viewDir = normalize(i.viewDir);
				// ����
				float rim = 1 - dot(worldNormal, viewDir); // 1- ����� ���� ����� 1
				// return _XRayColor * pow(rim,1 / _XRayPower);
				return _Color * pow(rim, 1 / _XRayPower);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
