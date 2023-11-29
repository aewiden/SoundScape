using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectInfo
{
    public GameObject prefab;
    public Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);

}
public class ObjectGenerator : MonoBehaviour
{
    public ObjectInfo[] objectLibrary;
    public Material[] materials;
    public int numofSpawns = 5;
    public float spawnInterval = 1.0f;
    public float growDuration = 1.0f;
    public float hold = 1.0f;
    public float shrinkDuration = 1.0f;
    public float scaleMultiplier = 0.1f;
    public float maxRotationSpeed = 180f;
    public float timer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnObjects();
            timer = 0.0f;
        }
    }

    void SpawnObjects()
    {
        for (int i = 0; i < numofSpawns; i++)
        {
            ObjectInfo objectInfo = objectLibrary[Random.Range(0, objectLibrary.Length)];

            Vector3 spawnPosition = RandomSpawnPosition();

            // Check if the spawn position is within the allowed range from the center
            if (Vector3.Distance(Vector3.zero, spawnPosition) < 4f)
                continue;

            GameObject spawned = Instantiate(objectInfo.prefab, spawnPosition, Random.rotation);
            spawned.transform.localScale = objectInfo.scale * scaleMultiplier;

            StartCoroutine(GrowHoldShrink(spawned, objectInfo.scale));

            Renderer objectRenderer = spawned.GetComponent<Renderer>();
            if (objectRenderer != null && materials.Length > 0)
            {
                Material randomMaterial = materials[Random.Range(0, materials.Length)];
                objectRenderer.material = randomMaterial;
            }
        }
    }

    IEnumerator GrowHoldShrink(GameObject obj, Vector3 targetScale)
    {
        float elapsedTime = 0f;
        Vector3 rotationAxis = Random.onUnitSphere;
        float rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);

        while (elapsedTime < growDuration)
        {
            obj.transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
            obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, targetScale, elapsedTime);
            elapsedTime += (Time.deltaTime * growDuration);
            yield return null;
        }

        yield return new WaitForSeconds(hold);

        elapsedTime = 0f;

        while(elapsedTime < shrinkDuration)
        {
            obj.transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
            obj.transform.localScale = Vector3.Lerp(targetScale, Vector3.zero, elapsedTime );
            elapsedTime += (Time.deltaTime * shrinkDuration);
            yield return null;
        }

        Destroy(obj);
    }

    Vector3 RandomSpawnPosition()
    {
        float x = Random.Range(-10f, 10f);
        float y = Random.Range(1f, 5f);
        float z = Random.Range(-10f, 10f);

        return new Vector3(x, y, z);
    }
}
