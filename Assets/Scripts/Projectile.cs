using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private Vector3 direction;
    private Vector3 velocity;
    private int collisionCount = 0;

    public ParticleSystem explosion;

    public Material decalMaterial;
    public Texture2D decalTexture;
    public Vector2 decalSize;

    private TextureSetup planeInitializer;
    public void Initialize(float power, Vector3 fireDirection)
    {
        speed = power;
        direction = fireDirection;
        velocity = direction * speed;

        CreateRandomMesh();
    }

    void Update()
    {
        velocity += Physics.gravity * Time.deltaTime;

        transform.position += velocity * Time.deltaTime;
    }

    void CreateRandomMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[8];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        }

        int[] triangles = new int[]
        {
            0, 2, 1,
            0, 3, 2,
            2, 3, 4,
            2, 4, 5,
            1, 2, 5,
            1, 5, 6,
            0, 7, 4,
            0, 4, 3,
            5, 4, 7,
            5, 7, 6,
            0, 6, 7,
            0, 1, 6
        };
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    void OnCollisionEnter(Collision collision)
    {
        collisionCount++;

        if (collisionCount == 1)
        {
            Vector3 reflectDirection = Vector3.Reflect(velocity.normalized, collision.contacts[0].normal);
            velocity = reflectDirection * speed;
        }
        else if (collisionCount >= 2)
        {
            if (explosion != null)
            {
                explosion.transform.position = transform.position;
                explosion.Play();
            }

            AddDecal(collision);
            Destroy(gameObject, 0.1f);
        }
    }

    void AddDecal(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Renderer renderer = collision.gameObject.GetComponent<Renderer>();

        if (renderer != null)
        {
            MeshCollider meshCollider = collision.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
                return;

            Vector2 uv;
            RaycastHit hit;
            Ray ray = new Ray(contact.point - contact.normal, contact.normal);
            if (collision.collider.Raycast(ray, out hit, 2))
            {
                uv = hit.textureCoord;
                Debug.Log($"UV Coordinates: {uv}");

                decalMaterial.SetTexture("_DecalTex", decalTexture);
                decalMaterial.SetVector("_DecalPos", new Vector4(uv.x, uv.y, 0, 0));
                decalMaterial.SetVector("_DecalSize", new Vector4(decalSize.x, decalSize.y, 1, 1));

                RenderTexture renderTexture = planeInitializer.GetRenderTexture();
                if (renderTexture != null)
                {
                    RenderTexture bufferTexture = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, 0, renderTexture.format);

                    Graphics.Blit(renderTexture, bufferTexture);
                    Graphics.Blit(bufferTexture, renderTexture, decalMaterial);

                    RenderTexture.ReleaseTemporary(bufferTexture);
                }
            }
        }
    }
}
