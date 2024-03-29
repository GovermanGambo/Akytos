#type vertex
#version 450

layout(location = 0) in vec3 a_Position;
layout(location = 1) in vec4 a_Color;
layout(location = 2) in vec2 a_UV;
layout(location = 3) in int a_TextureIndex;
layout(location = 4) in int a_ObjectId;

uniform mat4 u_ViewProjection;

out vec4 v_Color;
out vec2 v_UV;
out flat int v_TextureIndex;
out flat int v_ObjectId;

void main() {
    v_Color = a_Color;
    v_UV = a_UV;
    v_TextureIndex = a_TextureIndex;
    v_ObjectId = a_ObjectId;
    gl_Position = u_ViewProjection * vec4(a_Position, 1.0);
}

#type fragment
#version 450

layout(location = 0) out vec4 color;
layout(location = 1) out int color2;

in vec4 v_Color;
in vec2 v_UV;
in flat int v_TextureIndex;
in flat int v_ObjectId;

uniform sampler2D u_Textures[32];

void main() {
    vec4 texColor = v_Color;

	switch(v_TextureIndex)
	{
		case  0: texColor *= texture(u_Textures[ 0], v_UV); break;
		case  1: texColor *= texture(u_Textures[ 1], v_UV); break;
		case  2: texColor *= texture(u_Textures[ 2], v_UV); break;
		case  3: texColor *= texture(u_Textures[ 3], v_UV); break;
		case  4: texColor *= texture(u_Textures[ 4], v_UV); break;
		case  5: texColor *= texture(u_Textures[ 5], v_UV); break;
		case  6: texColor *= texture(u_Textures[ 6], v_UV); break;
		case  7: texColor *= texture(u_Textures[ 7], v_UV); break;
		case  8: texColor *= texture(u_Textures[ 8], v_UV); break;
		case  9: texColor *= texture(u_Textures[ 9], v_UV); break;
		case 10: texColor *= texture(u_Textures[10], v_UV); break;
		case 11: texColor *= texture(u_Textures[11], v_UV); break;
		case 12: texColor *= texture(u_Textures[12], v_UV); break;
		case 13: texColor *= texture(u_Textures[13], v_UV); break;
		case 14: texColor *= texture(u_Textures[14], v_UV); break;
		case 15: texColor *= texture(u_Textures[15], v_UV); break;
		case 16: texColor *= texture(u_Textures[16], v_UV); break;
		case 17: texColor *= texture(u_Textures[17], v_UV); break;
		case 18: texColor *= texture(u_Textures[18], v_UV); break;
		case 19: texColor *= texture(u_Textures[19], v_UV); break;
		case 20: texColor *= texture(u_Textures[20], v_UV); break;
		case 21: texColor *= texture(u_Textures[21], v_UV); break;
		case 22: texColor *= texture(u_Textures[22], v_UV); break;
		case 23: texColor *= texture(u_Textures[23], v_UV); break;
		case 24: texColor *= texture(u_Textures[24], v_UV); break;
		case 25: texColor *= texture(u_Textures[25], v_UV); break;
		case 26: texColor *= texture(u_Textures[26], v_UV); break;
		case 27: texColor *= texture(u_Textures[27], v_UV); break;
		case 28: texColor *= texture(u_Textures[28], v_UV); break;
		case 29: texColor *= texture(u_Textures[29], v_UV); break;
		case 30: texColor *= texture(u_Textures[30], v_UV); break;
		case 31: texColor *= texture(u_Textures[31], v_UV); break;
	}
	color = texColor;
	color2 = v_ObjectId;
}