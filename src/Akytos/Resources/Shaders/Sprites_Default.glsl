#type vertex
#version 450

layout(location = 0) in vec3 a_Position;
layout(location = 1) in vec4 a_Color;
layout(location = 2) in vec2 a_UV;
layout(location = 3) in int a_TextureIndex;
layout(location = 4) in int a_ObjectId;

layout(set = 0, binding = 0) uniform u_ViewProjection
{
	mat4 view_projection_matrix;
};

layout(location = 0) out vec4 v_Color;
layout(location = 1) out vec2 v_UV;
layout(location = 2) out flat int v_TextureIndex;
layout(location = 3) out flat int v_ObjectId;

void main() {
    v_Color = a_Color;
    v_UV = a_UV;
    v_TextureIndex = a_TextureIndex;
    v_ObjectId = a_ObjectId;
    gl_Position = view_projection_matrix * vec4(a_Position, 1.0);
}

#type fragment
#version 450

layout(location = 0) out vec4 color;
layout(location = 1) out int color2;

layout(location = 0) in vec4 v_Color;
layout(location = 1) in vec2 v_UV;
layout(location = 2) in flat int v_TextureIndex;
layout(location = 3) in flat int v_ObjectId;

layout(set = 0, binding = 1) uniform sampler2D u_Texture0;
layout(set = 0, binding = 2) uniform sampler2D u_Texture1;
layout(set = 0, binding = 3) uniform sampler2D u_Texture2;
layout(set = 0, binding = 4) uniform sampler2D u_Texture3;
layout(set = 0, binding = 5) uniform sampler2D u_Texture4;
layout(set = 0, binding = 6) uniform sampler2D u_Texture5;
layout(set = 0, binding = 7) uniform sampler2D u_Texture6;
layout(set = 0, binding = 8) uniform sampler2D u_Texture7;
layout(set = 0, binding = 9) uniform sampler2D u_Texture8;
layout(set = 0, binding = 10) uniform sampler2D u_Texture9;
layout(set = 0, binding = 11) uniform sampler2D u_Texture10;
layout(set = 0, binding = 12) uniform sampler2D u_Texture11;
layout(set = 0, binding = 13) uniform sampler2D u_Texture12;
layout(set = 0, binding = 14) uniform sampler2D u_Texture13;
layout(set = 0, binding = 15) uniform sampler2D u_Texture14;
layout(set = 0, binding = 16) uniform sampler2D u_Texture15;

void main() {
    vec4 texColor = v_Color;

	switch(v_TextureIndex)
	{
		case  0: texColor *= texture(u_Texture0, v_UV); break;
		case  1: texColor *= texture(u_Texture1, v_UV); break;
		case  2: texColor *= texture(u_Texture2, v_UV); break;
		case  3: texColor *= texture(u_Texture3, v_UV); break;
		case  4: texColor *= texture(u_Texture4, v_UV); break;
		case  5: texColor *= texture(u_Texture5, v_UV); break;
		case  6: texColor *= texture(u_Texture6, v_UV); break;
		case  7: texColor *= texture(u_Texture7, v_UV); break;
		case  8: texColor *= texture(u_Texture8, v_UV); break;
		case  9: texColor *= texture(u_Texture9, v_UV); break;
		case 10: texColor *= texture(u_Texture10, v_UV); break;
		case 11: texColor *= texture(u_Texture11, v_UV); break;
		case 12: texColor *= texture(u_Texture12, v_UV); break;
		case 13: texColor *= texture(u_Texture13, v_UV); break;
		case 14: texColor *= texture(u_Texture14, v_UV); break;
		case 15: texColor *= texture(u_Texture15, v_UV); break;
	}
	color = texColor;
	color2 = v_ObjectId;
}