using UnityEngine;

public class ParalaxWithMaterial : MonoBehaviour
{
    [SerializeField] private float speedX, speedY;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Material mat;
    private Vector2 offset;
    
    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (mat == null && spriteRenderer.sprite != null)
            mat = spriteRenderer.material;
    }

    void Update()
    {
        offset += new Vector2(speedX * Time.deltaTime, speedY * Time.deltaTime);
        mat.mainTextureOffset = offset;
    }
}
