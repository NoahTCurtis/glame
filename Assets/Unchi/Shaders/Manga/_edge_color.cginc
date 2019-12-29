uniform fixed _InkSize;
uniform fixed4 _InkColor;

// Vertex shader
struct VS {
  half4 Pos: POSITION;
};

// Vertex shader
VS vert (appdata_base i) {

  const fixed incDist = 1.1f;

  half4 viewNormal = mul (UNITY_MATRIX_MV, normalize (half4 (i.normal, 0)));
  half4 edgePos = mul (UNITY_MATRIX_MV, i.vertex);

  half4 edgeProj = mul (UNITY_MATRIX_P, edgePos);

  half distToCamera = clamp (length (edgePos) / incDist, 0.0, 1.0);
  half normalScale = lerp (0.1, 1.0, distToCamera) * _InkSize * 0.001;

  VS o;
  o.Pos = mul (UNITY_MATRIX_P, edgePos + normalScale * viewNormal * edgeProj.z);
  return o;
}
// Fragment shader
fixed4 frag (VS i): COLOR {
  return _InkColor;
}

