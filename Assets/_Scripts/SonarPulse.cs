using UnityEngine;

public class SonarPulse : MonoBehaviour
{
    public float maxScale = 40f;        
    public float expansionSpeed = 15f;   
    public float fadeOutSpeed = 150f;    
    public float fadeOutDuration = 1f; 
    public Material sonarMaterial;      
    public LayerMask detectionLayer;   

    private float currentScale = 0f;
    private bool isFadingOut = false;
    private Renderer sonarRenderer;
    private float fadeOutStartTime;

    void Start()
    {
        sonarRenderer = GetComponent<Renderer>();

        sonarMaterial = new Material(sonarMaterial);
        sonarRenderer.material = sonarMaterial;

        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (!isFadingOut)
        {
            currentScale += expansionSpeed * Time.deltaTime;
            transform.localScale = Vector3.one * currentScale;

            CheckForObjectHit();

            if (currentScale >= maxScale)
            {
                currentScale = maxScale;
                isFadingOut = true;
                fadeOutStartTime = Time.time;
            }
        }
        else
        {
            currentScale -= fadeOutSpeed * Time.deltaTime;
            transform.localScale = Vector3.one * currentScale;

            if (currentScale <= 0.01f)
            {
                Destroy(gameObject);
            }
        }
    }

    void CheckForObjectHit()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, currentScale / 2, detectionLayer);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                sonarMaterial.SetColor("_EdgeGlow", Color.red);
                ActivateOutline activateOutline = hitCollider.GetComponent<ActivateOutline>();
                activateOutline.Activate();
                EnemyAI enemyAI = hitCollider.GetComponent<EnemyAI>();
                if (enemyAI != null)
                    {
                        enemyAI.TriggerChase();
                    }
            }

            if (hitCollider.CompareTag("Trap"))
            {
                sonarMaterial.SetColor("_EdgeGlow", Color.yellow);
                ActivateOutline activateOutline = hitCollider.GetComponent<ActivateOutline>();
                activateOutline.Activate();
            }

            if (hitCollider.CompareTag("WallTrap"))
            {
                sonarMaterial.SetColor("_EdgeGlow", Color.grey);
                ActivateOutline activateOutline = hitCollider.GetComponent<ActivateOutline>();
                activateOutline.Activate();
                MovingWall movingWall = hitCollider.GetComponent<MovingWall>();
                if (movingWall != null)
                    {
                        movingWall.MoveWall();
                    }
            }
            
            if (hitCollider.CompareTag("Key"))
            {
                sonarMaterial.SetColor("_EdgeGlow", Color.cyan);
                ActivateOutline activateOutline = hitCollider.GetComponent<ActivateOutline>();
                activateOutline.Activate();
            }

            if (hitCollider.CompareTag("Door"))
            {
                sonarMaterial.SetColor("_EdgeGlow", Color.white);
                ActivateOutline activateOutline = hitCollider.GetComponent<ActivateOutline>();
                activateOutline.Activate();
            }

            if (hitCollider.CompareTag("Platform"))
            {
                sonarMaterial.SetColor("_EdgeGlow", Color.grey);
                ActivateOutline activateOutline = hitCollider.GetComponent<ActivateOutline>();
                activateOutline.Activate();    
            }
        }
    }
}
