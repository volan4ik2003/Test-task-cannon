using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCreator : MonoBehaviour
{
    public Texture2D bulletMarkTexture;
    public float bulletMarkSize = 0.1f;

    private void OnCollisionEnter(Collision collision)
    {
        Renderer renderer = collision.gameObject.GetComponent<Renderer>();
        if (renderer != null && renderer.material.HasProperty("_BulletMark"))
        {
            Vector2 uv = GetCollisionUV(collision);
            DrawBulletMark(renderer, uv);
        }
    }

    private Vector2 GetCollisionUV(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Vector2 uv = Vector2.zero;
        if (collision.collider is MeshCollider)
        {
            MeshCollider meshCollider = (MeshCollider)collision.collider;
            Mesh mesh = meshCollider.sharedMesh;
            Ray ray = new Ray(contact.point - contact.normal * 0.01f, contact.normal);
            RaycastHit hit;
            if (meshCollider.Raycast(ray, out hit, 1.0f))
            {
                uv = hit.textureCoord;
            }
        }
        return uv;
    }

    private void DrawBulletMark(Renderer renderer, Vector2 uv)
    {
        Material material = renderer.material;
        Texture2D texture = Instantiate(material.GetTexture("_BulletMark")) as Texture2D;

        int x = (int)(uv.x * texture.width);
        int y = (int)(uv.y * texture.height);
        int halfSize = (int)(bulletMarkSize * texture.width / 2);

        for (int i = -halfSize; i < halfSize; i++)
        {
            for (int j = -halfSize; j < halfSize; j++)
            {
                texture.SetPixel(x + i, y + j, bulletMarkTexture.GetPixel(i + halfSize, j + halfSize));
            }
        }

        texture.Apply();
        material.SetTexture("_BulletMark", texture);
    }
}
