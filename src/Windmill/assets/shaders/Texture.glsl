#type vertex
#version 450

layout(location = 0) in vec3 a_Position;
layout(location = 1) in vec2 a_UV;

out vec2 v_UV;

uniform mat4 u_ViewProjection;
uniform mat4 u_Transform;

void main() {
    v_UV = a_UV;
    gl_Position = u_ViewProjection * u_Transform * vec4(a_Position, 1.0f);
}

#type fragment
#version 450

layout(location = 0) out vec4 color;

in vec2 v_UV;

uniform sampler2D u_Texture;

void main() {
    color = texture(u_Texture, v_UV);
}