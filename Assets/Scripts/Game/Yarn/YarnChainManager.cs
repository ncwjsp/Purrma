using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using System;

public class YarnChainManager : MonoBehaviour
{
    [Header("Chain Movement Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float yarnSpacing = 0.3f;
    
    [Header("Gap Animation Settings")]
    [SerializeField] private float frontSlowSpeed = 0.0f; // Speed when front part is waiting for back part (completely stopped)
    [SerializeField] private float gapThreshold = 0.5f; // Minimum gap size to trigger animation
    [SerializeField] private float catchUpSpeed = 6.0f; // Speed multiplier for back part catching up (much faster)
    [SerializeField] private float animationDuration = 1.5f; // How long the animation should last (shorter for snappier effect)
    
    [Header("Chain Setup")]
    [SerializeField] private SplineContainer spline;
    [SerializeField] private GameObject[] yarnPrefabs;
    [SerializeField] private int initialCount = 10;

    [Header("Stage Patterns")]
    [SerializeField] private YarnPattern[] stagePatterns;
    [SerializeField] private int currentStage = 0;

    private float pathLength;
    private List<GameObject> yarns = new List<GameObject>();
    private List<float> distances = new List<float>();
    
    // Gap animation state
    private bool isGapAnimationActive = false;
    private int gapStartIndex = -1; // Index where the gap starts (front part)
    private int gapEndIndex = -1; // Index where the gap ends (back part)
    private float animationStartTime = 0f; // When the animation started
    private float originalGapSize = 0f; // Original gap size when animation started
    
    // Match processing state
    private bool isProcessingMatches = false;
    private Queue<int> matchQueue = new Queue<int>();
    
    [Header("Game Over Settings")]
    [SerializeField] private bool gameOverEnabled = true;
    [SerializeField] private float gameOverThreshold = 0.95f; // Game over when yarn reaches 95% of path
    private bool isGameOver = false;
    private bool isWin = false;
    
    [Header("Audio")]
    [SerializeField] private AudioSource matchSound; // Sound effect for matching 3+ yarns
    
    [Header("Match Animation Settings")]
    [SerializeField] private float matchDelay = 0.5f; // Delay between matches in a combo

    private void Start()
    {
        pathLength = spline.CalculateLength();
        CreateDefaultPatterns();
        
        // Auto-detect stage based on scene name
        AutoDetectStage();
        
        LoadStagePattern();
    }

    private void Update()
    {
        // Don't update if game is over
        if (isGameOver) return;
        
        // Clean up any destroyed yarns first
        CleanupDestroyedYarns();
        
        // Process match queue
        ProcessMatchQueue();
        
        // Check for gaps and update animation state
        CheckForGaps();
        
        // Move all yarns along the spline
        for (int i = 0; i < distances.Count; i++)
        {
            // Skip if yarn is null (destroyed)
            if (yarns[i] == null) continue;
            
            // Calculate movement speed based on gap animation
            float currentSpeed = CalculateMovementSpeed(i);
            distances[i] += currentSpeed * Time.deltaTime;
            
            float t = distances[i] / pathLength;
            t = Mathf.Clamp01(t);

            Vector3 pos = spline.EvaluatePosition(t);
            Vector3 tangent = spline.EvaluateTangent(t);

            yarns[i].transform.position = pos;
            float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
            yarns[i].transform.rotation = Quaternion.Euler(0, 0, angle);

            // keep each node index in sync
            var node = yarns[i].GetComponent<YarnChainNode>();
            if (node != null)
            {
                node.index = i;
            }
            
            // Check for game over condition
            if (gameOverEnabled && t >= gameOverThreshold && !isGameOver)
            {
                Debug.Log($"Game Over! Yarn at index {i} reached the end of the path (t={t:F2})");
                TriggerGameOver();
                break;
            }
        }
        
        // Debug: Log positions every few seconds
        if (Time.time % 2f < Time.deltaTime && yarns.Count > 0)
        {
            Debug.Log($"Yarn positions - Count: {yarns.Count}, First distance: {distances[0]}, Last distance: {distances[distances.Count-1]}, Gap animation: {isGapAnimationActive}");
        }
    }

    // <- THIS is where your method goes
    private void SpawnYarn(float dist)
    {
        GameObject prefab = yarnPrefabs[UnityEngine.Random.Range(0, yarnPrefabs.Length)];
        GameObject yarn = Instantiate(prefab, spline.EvaluatePosition(dist / pathLength), Quaternion.identity);

        // Add YarnChainNode so we can detect collisions
        var node = yarn.AddComponent<YarnChainNode>();
        node.chain = this;
        node.index = yarns.Count;

        // Ensure the yarn has a Yarn component and set its color
        var yarnComponent = yarn.GetComponent<Yarn>();
        if (yarnComponent == null)
        {
            yarnComponent = yarn.AddComponent<Yarn>();
        }
        
        // Set a random color for the yarn based on prefab name
        string randomColor = GetColorFromPrefabName(prefab.name);
        yarnComponent.SetColor(randomColor);
        Debug.Log($"Spawned yarn at index {yarns.Count} with color: {randomColor} (from prefab: {prefab.name})");

        // Ensure the yarn has a Rigidbody2D for proper physics collision
        if (yarn.GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb = yarn.AddComponent<Rigidbody2D>();
            rb.isKinematic = true; // Kinematic so we can control it manually but still get collision detection
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            Debug.Log($"Added Rigidbody2D to yarn at index {yarns.Count}");
        }

        // Ensure the yarn has a collider for bullet detection
        if (yarn.GetComponent<Collider2D>() == null)
        {
            CircleCollider2D collider = yarn.AddComponent<CircleCollider2D>();
            collider.isTrigger = false; // Not a trigger - we want solid collision
            collider.radius = 0.5f; // Match bullet collider size
            Debug.Log($"Added collider to yarn at index {yarns.Count}");
        }
        else
        {
            // Make sure existing collider is not a trigger
            var existingCollider = yarn.GetComponent<Collider2D>();
            if (existingCollider != null)
            {
                existingCollider.isTrigger = false;
                if (existingCollider is CircleCollider2D circleCollider)
                {
                    circleCollider.radius = 0.5f; // Match bullet collider size
                }
                Debug.Log($"Set existing collider to non-trigger for yarn at index {yarns.Count}");
            }
        }

        yarns.Add(yarn);
        distances.Add(dist);
    }

    // You'll also put InsertYarnAt() here later
    public void InsertYarnAt(YarnChainNode hitNode, GameObject newYarn)
{
    int insertIndex = hitNode.index; // insert BEFORE the yarn we hit
    Debug.Log($"Inserting yarn at index {insertIndex}, total yarns before: {yarns.Count}");

    // Figure out where this yarn should sit along the path
    // Use the exact position of the hit node, not a calculated distance
    float insertDist = distances[insertIndex];
    Debug.Log($"Insert distance: {insertDist}, yarnSpacing: {yarnSpacing}");

    // Shift all following yarns back to make space
    for (int i = insertIndex; i < distances.Count; i++)
    {
        distances[i] += yarnSpacing;
        Debug.Log($"Shifted yarn {i} distance to {distances[i]}");
    }

    // Insert into the lists
    yarns.Insert(insertIndex, newYarn);
    distances.Insert(insertIndex, insertDist);

    // Attach YarnChainNode so this yarn becomes part of the chain
    var node = newYarn.GetComponent<YarnChainNode>();
    if (node == null) node = newYarn.AddComponent<YarnChainNode>();
    node.chain = this;
    node.index = insertIndex;

    // Ensure the inserted yarn has a Rigidbody2D for proper physics collision
    if (newYarn.GetComponent<Rigidbody2D>() == null)
    {
        Rigidbody2D rb = newYarn.AddComponent<Rigidbody2D>();
        rb.isKinematic = true; // Kinematic so we can control it manually but still get collision detection
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        Debug.Log($"Added Rigidbody2D to inserted yarn at index {insertIndex}");
    }

    // Ensure the inserted yarn has a collider for future bullet detection
    if (newYarn.GetComponent<Collider2D>() == null)
    {
        CircleCollider2D collider = newYarn.AddComponent<CircleCollider2D>();
        collider.isTrigger = false; // Not a trigger - we want solid collision
        collider.radius = 0.5f; // Match bullet collider size
        Debug.Log($"Added collider to inserted yarn at index {insertIndex}");
    }
    else
    {
        // Make sure existing collider is not a trigger
        var existingCollider = newYarn.GetComponent<Collider2D>();
        if (existingCollider != null)
        {
            existingCollider.isTrigger = false;
            if (existingCollider is CircleCollider2D circleCollider)
            {
                circleCollider.radius = 0.5f; // Match bullet collider size
            }
            Debug.Log($"Set existing collider to non-trigger for inserted yarn at index {insertIndex}");
        }
    }

    // Parent to chain object for organization
    newYarn.transform.SetParent(this.transform);

    // Snap visually to spline position
    Vector3 pos = spline.EvaluatePosition(insertDist / pathLength);
    Vector3 tangent = spline.EvaluateTangent(insertDist / pathLength);
    newYarn.transform.position = pos;
    float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
    newYarn.transform.rotation = Quaternion.Euler(0, 0, angle);

    // Re-index all following nodes so they have the correct index
    for (int i = insertIndex + 1; i < yarns.Count; i++)
    {
        yarns[i].GetComponent<YarnChainNode>().index = i;
    }

    // Check for 3+ matches and queue them for processing
    Debug.Log("🚀 INSERT YARN COMPLETE - Starting match detection...");
    QueueAllMatches();
}



private void CloseGapsAfterRemoval(int removedStartIndex, int removedCount)
{
    // Don't immediately close gaps - let the animation system handle it
    // Just update the indices
    for (int i = removedStartIndex; i < distances.Count; i++)
    {
        // Update the index of the yarn node
        if (i < yarns.Count && yarns[i] != null)
        {
            var node = yarns[i].GetComponent<YarnChainNode>();
            if (node != null)
            {
                node.index = i;
            }
        }
    }
    
    Debug.Log($"Gap created: {removedCount} yarns removed, gap animation will handle closing");
    
    // Trigger gap animation to start checking for gaps
    TriggerGapAnimation();
    
    // Cascading matches are now handled by the queue system
}

    private void QueueAllMatches()
    {
        bool foundAnyMatch = true;
        int totalMatches = 0;
        
        Debug.Log($"🔍 QUEUEING MATCHES... Total yarns: {yarns.Count}");
        
        // Clear existing queue first
        matchQueue.Clear();
        
        // Keep checking until no more matches are found
        while (foundAnyMatch && totalMatches < 50) // Increased safety limit
        {
            foundAnyMatch = false;
            
            // Check every possible 3-yarn sequence in the chain
            for (int i = 0; i <= yarns.Count - 3; i++)
            {
                if (yarns[i] != null && yarns[i + 1] != null && yarns[i + 2] != null)
                {
                    var yarn1 = yarns[i].GetComponent<Yarn>();
                    var yarn2 = yarns[i + 1].GetComponent<Yarn>();
                    var yarn3 = yarns[i + 2].GetComponent<Yarn>();
                    
                    if (yarn1 != null && yarn2 != null && yarn3 != null)
                    {
                        if (yarn1.colorName == yarn2.colorName && 
                            yarn2.colorName == yarn3.colorName)
                        {
                            Debug.Log($"✅ QUEUEING MATCH at indices {i}, {i+1}, {i+2}: {yarn1.colorName}");
                            
                            // Add to queue instead of processing immediately
                            matchQueue.Enqueue(i);
                            foundAnyMatch = true;
                            totalMatches++;
                            break; // Start checking from the beginning again
                        }
                    }
                }
            }
        }
        
        if (totalMatches >= 50)
        {
            Debug.LogWarning($"Reached maximum match checks (50). Found {totalMatches} matches total.");
        }
        else
        {
            Debug.Log($"🎯 MATCH QUEUEING COMPLETE: Found and queued {totalMatches} matches. Queue size: {matchQueue.Count}");
        }
    }
    
    private float lastMatchTime = 0f;
    
    private void ProcessMatchQueue()
    {
        if (matchQueue.Count == 0) return;
        
        float timeSinceLastMatch = Time.time - lastMatchTime;
        Debug.Log($"⏰ Queue has {matchQueue.Count} matches. Time since last: {timeSinceLastMatch:F2}s, need: {matchDelay}s");
        
        // Check if enough time has passed since last match
        if (timeSinceLastMatch >= matchDelay)
        {
            int matchIndex = matchQueue.Dequeue();
            Debug.Log($"🎯 PROCESSING MATCH from queue: index {matchIndex}, queue remaining: {matchQueue.Count}");
            ProcessSingleMatch(matchIndex);
            lastMatchTime = Time.time;
            
            // Check for win condition after each match
            CheckForWinCondition();
        }
    }
    
    private void ProcessSingleMatch(int startIndex)
    {
        if (yarns[startIndex] == null) return;
        
        string color = yarns[startIndex].GetComponent<Yarn>().colorName;
        Debug.Log($"Processing match starting at index {startIndex} with color {color}");

        int left = startIndex;
        int right = startIndex;

        // Expand left
        while (left > 0 && yarns[left - 1] != null && 
               yarns[left - 1].GetComponent<Yarn>().colorName == color)
            left--;

        // Expand right
        while (right < yarns.Count - 1 && yarns[right + 1] != null && 
               yarns[right + 1].GetComponent<Yarn>().colorName == color)
            right++;

        int count = right - left + 1;
        Debug.Log($"Found {count} consecutive yarns of color {color} from index {left} to {right}");
        
        if (count >= 3)
        {
            Debug.Log($"DESTROYING {count} yarns of color {color} at indices {left} to {right}");
            
            // Play match sound effect
            if (matchSound != null)
            {
                matchSound.Play();
            }
            
            for (int i = right; i >= left; i--)
            {
                if (yarns[i] != null)
                {
                    Debug.Log($"Destroying yarn at index {i}");
                    Destroy(yarns[i]);
                    yarns.RemoveAt(i);
                    distances.RemoveAt(i);
                }
            }
            
            // Close gaps after removing matched yarns
            CloseGapsAfterRemoval(left, count);
            
            // Trigger gap animation after matching
            TriggerGapAnimation();
            
            // Check for new matches that might have been created and queue them
            QueueAllMatches();
        }
    }

private void CleanupDestroyedYarns()
{
    // Remove null entries from both lists
    for (int i = yarns.Count - 1; i >= 0; i--)
    {
        if (yarns[i] == null)
        {
            yarns.RemoveAt(i);
            distances.RemoveAt(i);
            Debug.Log($"Removed destroyed yarn at index {i}");
        }
    }
    
    // Check for win condition after cleanup
    CheckForWinCondition();
}

private void FixInitialMatches()
{
    string[] availableColors = {"Red", "Green", "Blue"};
    bool foundMatch = true;
    int fixCount = 0;
    
    Debug.Log($"Starting to fix initial matches. Total yarns: {yarns.Count}");
    
    // Log all current colors for debugging
    for (int i = 0; i < yarns.Count; i++)
    {
        if (yarns[i] != null)
        {
            var yarn = yarns[i].GetComponent<Yarn>();
            if (yarn != null)
            {
                Debug.Log($"Yarn {i}: {yarn.colorName}");
            }
        }
    }
    
    // Keep checking and fixing until no more 3+ matches exist
    while (foundMatch && fixCount < 20) // Increased safety limit
    {
        foundMatch = false;
        
        // Check for 3+ consecutive matches
        for (int i = 0; i <= yarns.Count - 3; i++)
        {
            if (yarns[i] != null && yarns[i + 1] != null && yarns[i + 2] != null)
            {
                var yarn1 = yarns[i].GetComponent<Yarn>();
                var yarn2 = yarns[i + 1].GetComponent<Yarn>();
                var yarn3 = yarns[i + 2].GetComponent<Yarn>();
                
                if (yarn1 != null && yarn2 != null && yarn3 != null)
                {
                    Debug.Log($"Checking yarns {i}, {i+1}, {i+2}: {yarn1.colorName}, {yarn2.colorName}, {yarn3.colorName}");
                    
                    if (yarn1.colorName == yarn2.colorName && 
                        yarn2.colorName == yarn3.colorName)
                    {
                        // Found a 3+ match, change the middle yarn to break it
                        string currentColor = yarn2.colorName;
                        string newColor = GetRandomDifferentColor(currentColor);
                        
                        yarn2.SetColor(newColor);
                        Debug.Log($"FIXED MATCH: Changed yarn at index {i + 1} from {currentColor} to {newColor}");
                        foundMatch = true;
                        fixCount++;
                        break; // Start checking from the beginning again
                    }
                }
            }
        }
    }
    
    if (fixCount >= 20)
    {
        Debug.LogWarning("Reached maximum fix attempts (20). Some matches may remain.");
    }
    else
    {
        Debug.Log($"Initial matches fixed successfully! Made {fixCount} changes. No 3+ consecutive matches remain.");
    }
}

private string GetColorFromPrefabName(string prefabName)
{
    // Extract color from prefab name (assuming prefabs are named like "Yarn_Red", "Yarn_Green", etc.)
    if (prefabName.Contains("Red") || prefabName.Contains("red"))
        return "Red";
    else if (prefabName.Contains("Green") || prefabName.Contains("green"))
        return "Green";
    else if (prefabName.Contains("Blue") || prefabName.Contains("blue"))
        return "Blue";
    else if (prefabName.Contains("Pink") || prefabName.Contains("pink"))
        return "Pink";
    else if (prefabName.Contains("Purple") || prefabName.Contains("purple"))
        return "Purple";
    else if (prefabName.Contains("Yellow") || prefabName.Contains("yellow"))
        return "Yellow";
    else
    {
        // Fallback to random color if we can't determine from name
        string[] colors = {"Red", "Green", "Blue", "Pink", "Purple", "Yellow"};
        return colors[UnityEngine.Random.Range(0, colors.Length)];
    }
}

private string GetRandomDifferentColor(string currentColor)
{
    string[] availableColors = {"Red", "Green", "Blue", "Pink", "Purple", "Yellow"};
    string newColor = availableColors[UnityEngine.Random.Range(0, availableColors.Length)];
    
    // Make sure we pick a different color
    while (newColor == currentColor && availableColors.Length > 1)
    {
        newColor = availableColors[UnityEngine.Random.Range(0, availableColors.Length)];
    }
    
    return newColor;
}

private void CreateDefaultPatterns()
{
    // Only create default patterns if none are defined
    if (stagePatterns != null && stagePatterns.Length > 0) return;
    
    Debug.Log("Creating default stage patterns...");
    
    stagePatterns = new YarnPattern[3];
    
    // Stage 0: Easy - Basic pattern with all colors (20 yarns)
    stagePatterns[0] = new YarnPattern
    {
        patternName = "Easy - Color Introduction",
        yarnColors = new string[] { "Red", "Blue", "Green", "Yellow", "Pink", "Purple", "Red", "Blue", "Green", "Yellow", "Pink", "Purple", "Red", "Blue", "Green", "Yellow", "Pink", "Purple", "Red", "Blue" },
        description = "Learn all the colors. Simple pattern to get familiar with the game.",
        hasMatches = false,
        difficulty = 1
    };
    
    // Stage 1: Medium - Strategic matches (25 yarns)
    stagePatterns[1] = new YarnPattern
    {
        patternName = "Medium - Strategic Matches",
        yarnColors = new string[] { "Red", "Red", "Blue", "Green", "Green", "Yellow", "Pink", "Pink", "Purple", "Red", "Blue", "Blue", "Green", "Yellow", "Yellow", "Pink", "Purple", "Purple", "Red", "Blue", "Green", "Yellow", "Pink", "Purple", "Red" },
        description = "Almost matches! One shot can create 3+ matches. Plan your strategy carefully.",
        hasMatches = true,
        difficulty = 2
    };
    
    // Stage 2: Hard - Complex challenge (30 yarns)
    stagePatterns[2] = new YarnPattern
    {
        patternName = "Hard - Master Challenge",
        yarnColors = new string[] { "Red", "Red", "Blue", "Green", "Green", "Yellow", "Pink", "Pink", "Purple", "Red", "Blue", "Blue", "Green", "Yellow", "Yellow", "Pink", "Purple", "Purple", "Red", "Blue", "Green", "Yellow", "Pink", "Purple", "Red", "Blue", "Green", "Yellow", "Pink", "Purple" },
        description = "Ultimate challenge! Multiple match opportunities with long chain. Master level difficulty.",
        hasMatches = true,
        difficulty = 3
    };
    
    Debug.Log($"Created {stagePatterns.Length} default patterns");
}

private void AutoDetectStage()
{
    string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.ToLower();
    
    if (sceneName.Contains("easy"))
    {
        currentStage = 0; // Easy stage
        Debug.Log("Auto-detected Easy stage (Stage 0)");
    }
    else if (sceneName.Contains("medium"))
    {
        currentStage = 1; // Medium stage
        Debug.Log("Auto-detected Medium stage (Stage 1)");
    }
    else if (sceneName.Contains("hard"))
    {
        currentStage = 2; // Hard stage
        Debug.Log("Auto-detected Hard stage (Stage 2)");
    }
    else
    {
        Debug.LogWarning($"Could not auto-detect stage from scene name: {sceneName}. Using default stage 0.");
        currentStage = 0;
    }
}

private void LoadStagePattern()
{
    // Always create default patterns if none are defined
    if (stagePatterns == null || stagePatterns.Length == 0)
    {
        Debug.Log("No stage patterns defined! Creating default patterns.");
        CreateDefaultPatterns();
    }
    
    if (currentStage >= stagePatterns.Length)
    {
        Debug.LogWarning($"Stage {currentStage} not found! Using last available pattern.");
        currentStage = stagePatterns.Length - 1;
    }
    
    if (currentStage < 0)
    {
        Debug.LogWarning($"Stage {currentStage} is invalid! Using first stage.");
        currentStage = 0;
    }
    
    YarnPattern pattern = stagePatterns[currentStage];
    Debug.Log($"Loading stage {currentStage}: {pattern.patternName}");
    Debug.Log($"Pattern has {pattern.yarnColors.Length} yarns: {string.Join(", ", pattern.yarnColors)}");
    
    // Clear existing yarns
    foreach (GameObject yarn in yarns)
    {
        if (yarn != null) Destroy(yarn);
    }
    yarns.Clear();
    distances.Clear();
    
    // Spawn yarns according to pattern
    for (int i = 0; i < pattern.yarnColors.Length; i++)
    {
        float dist = i * yarnSpacing;
        SpawnYarnWithColor(dist, pattern.yarnColors[i]);
    }
    
    Debug.Log($"Loaded pattern with {pattern.yarnColors.Length} yarns");
}

private void LoadRandomPattern()
{
    // Fallback to random generation if no patterns are defined
    for (int i = 0; i < initialCount; i++)
    {
        float dist = i * yarnSpacing;
        SpawnYarn(dist);
    }
}

private void SpawnYarnWithColor(float dist, string colorName)
{
    // Find the appropriate prefab for this color
    GameObject prefab = GetPrefabForColor(colorName);
    if (prefab == null)
    {
        Debug.LogError($"No prefab found for color: {colorName}");
        return;
    }
    
    GameObject yarn = Instantiate(prefab, spline.EvaluatePosition(dist / pathLength), Quaternion.identity);

    // Add YarnChainNode so we can detect collisions
    var node = yarn.AddComponent<YarnChainNode>();
    node.chain = this;
    node.index = yarns.Count;

    // Ensure the yarn has a Yarn component and set its color
    var yarnComponent = yarn.GetComponent<Yarn>();
    if (yarnComponent == null)
    {
        yarnComponent = yarn.AddComponent<Yarn>();
    }
    
    yarnComponent.SetColor(colorName);
    Debug.Log($"Spawned yarn at index {yarns.Count} with color: {colorName}");

    // Ensure the yarn has a Rigidbody2D for proper physics collision
    if (yarn.GetComponent<Rigidbody2D>() == null)
    {
        Rigidbody2D rb = yarn.AddComponent<Rigidbody2D>();
        rb.isKinematic = true; // Kinematic so we can control it manually but still get collision detection
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    // Ensure the yarn has a collider for bullet detection
    if (yarn.GetComponent<Collider2D>() == null)
    {
        CircleCollider2D collider = yarn.AddComponent<CircleCollider2D>();
        collider.isTrigger = false; // Not a trigger - we want solid collision
        collider.radius = 0.5f; // Match bullet collider size
    }
    else
    {
        // Make sure existing collider is not a trigger
        var existingCollider = yarn.GetComponent<Collider2D>();
        if (existingCollider != null)
        {
            existingCollider.isTrigger = false;
            if (existingCollider is CircleCollider2D circleCollider)
            {
                circleCollider.radius = 0.5f; // Match bullet collider size
            }
        }
    }

    yarns.Add(yarn);
    distances.Add(dist);
}

private GameObject GetPrefabForColor(string colorName)
{
    // Find the prefab that matches this color
    foreach (GameObject prefab in yarnPrefabs)
    {
        if (prefab.name.Contains(colorName) || prefab.name.Contains(colorName.ToLower()))
        {
            return prefab;
        }
    }
    
    // Fallback to random prefab if no match found
    if (yarnPrefabs.Length > 0)
    {
        return yarnPrefabs[UnityEngine.Random.Range(0, yarnPrefabs.Length)];
    }
    
    return null;
}

public void NextStage()
{
    Debug.Log($"NextStage called! Current stage before increment: {currentStage}");
    currentStage++;
    Debug.Log($"Current stage after increment: {currentStage}");
    
    // Check if we've reached the end of available stages
    if (currentStage >= stagePatterns.Length)
    {
        Debug.Log("Congratulations! You've completed all stages! Restarting from stage 0.");
        currentStage = 0; // Loop back to the beginning
    }
    
    // Reset win state for new stage
    isWin = false;
    
    Debug.Log($"About to load stage: {currentStage}");
    LoadStagePattern();
}

public void SetStage(int stage)
{
    if (stage >= 0 && stage < stagePatterns.Length)
    {
        currentStage = stage;
        LoadStagePattern();
    }
    else
    {
        Debug.LogWarning($"Invalid stage {stage}. Valid stages are 0-{stagePatterns.Length - 1}");
    }
}

public int GetMaxStages()
{
    return stagePatterns != null ? stagePatterns.Length : 0;
}

public string GetStageName(int stage)
{
    if (stagePatterns != null && stage >= 0 && stage < stagePatterns.Length)
    {
        return stagePatterns[stage].patternName;
    }
    return "Unknown Stage";
}

public string GetStageDescription(int stage)
{
    if (stagePatterns != null && stage >= 0 && stage < stagePatterns.Length)
    {
        return stagePatterns[stage].description;
    }
    return "No description available";
}

public int GetStageDifficulty(int stage)
{
    if (stagePatterns != null && stage >= 0 && stage < stagePatterns.Length)
    {
        return stagePatterns[stage].difficulty;
    }
    return 1;
}

// Public methods to control chain movement speed
public void SetChainSpeed(float newSpeed)
{
    speed = newSpeed;
    Debug.Log($"Chain speed set to: {speed}");
}

public float GetChainSpeed()
{
    return speed;
}

// Method to get all active yarns for targeting
public List<GameObject> GetAllYarns()
{
    return new List<GameObject>(yarns);
}

// Method to get yarn at specific index
public GameObject GetYarnAt(int index)
{
    if (index >= 0 && index < yarns.Count)
    {
        return yarns[index];
    }
    return null;
}

// Method to gradually change speed over time
public void ChangeSpeedOverTime(float targetSpeed, float duration)
{
    StartCoroutine(ChangeSpeedCoroutine(targetSpeed, duration));
}

private System.Collections.IEnumerator ChangeSpeedCoroutine(float targetSpeed, float duration)
{
    float startSpeed = speed;
    float elapsed = 0f;
    
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        speed = Mathf.Lerp(startSpeed, targetSpeed, elapsed / duration);
        yield return null;
    }
    
    speed = targetSpeed;
    Debug.Log($"Chain speed changed to: {speed}");
}

    private void CheckForCascadingMatches(int startIndex)
    {
        // Check for matches around the area where gaps were closed
        int checkStart = Mathf.Max(0, startIndex - 2);
        int checkEnd = Mathf.Min(yarns.Count - 1, startIndex + 2);
        
        for (int i = checkStart; i <= checkEnd - 2; i++)
        {
            if (yarns[i] != null && yarns[i + 1] != null && yarns[i + 2] != null)
            {
                var yarn1 = yarns[i].GetComponent<Yarn>();
                var yarn2 = yarns[i + 1].GetComponent<Yarn>();
                var yarn3 = yarns[i + 2].GetComponent<Yarn>();
                
                if (yarn1 != null && yarn2 != null && yarn3 != null &&
                    yarn1.colorName == yarn2.colorName && 
                    yarn2.colorName == yarn3.colorName)
                {
                    Debug.Log($"Found cascading match at indices {i}, {i + 1}, {i + 2}");
                    ProcessSingleMatch(i);
                    break; // Check again after this match is processed
                }
            }
        }
    }

    // Game Over System
    public static event Action OnGameOver;
    public static event Action OnWin; // Event for when player wins
    
    private void TriggerGameOver()
    {
        if (isGameOver) return; // Prevent multiple triggers
        
        isGameOver = true;
        Debug.Log("Game Over triggered!");
        
        // Notify all listeners
        OnGameOver?.Invoke();
    }
    
    public bool IsGameOver()
    {
        return isGameOver;
    }
    
    public void ResetGame()
    {
        isGameOver = false;
        isWin = false;
        // Optionally reload the current stage
        LoadStagePattern();
    }
    
    public int GetRemainingYarns()
    {
        return yarns.Count;
    }
    
    public float GetGameProgress()
    {
        if (yarns.Count == 0) return 1f;
        
        float maxProgress = 0f;
        for (int i = 0; i < distances.Count; i++)
        {
            if (yarns[i] != null)
            {
                float progress = distances[i] / pathLength;
                maxProgress = Mathf.Max(maxProgress, progress);
            }
        }
        return maxProgress;
    }
    
    private void CheckForWinCondition()
    {
        // Check if all yarns have been cleared
        if (yarns.Count == 0 && !isGameOver && !isWin)
        {
            Debug.Log($"WIN! All yarns have been cleared! Current stage: {currentStage}");
            TriggerWin();
        }
    }
    
    private void TriggerWin()
    {
        if (isGameOver || isWin) return; // Prevent triggering if already game over or already won
        
        isWin = true;
        Debug.Log("Win triggered!");
        
        // Notify all listeners
        OnWin?.Invoke();
    }

    // Gap Animation Methods
    private void CheckForGaps()
    {
        if (yarns.Count < 2) 
        {
            isGapAnimationActive = false;
            return;
        }
        
        // If animation is already active, check if it should end
        if (isGapAnimationActive)
        {
            float elapsedTime = Time.time - animationStartTime;
            
            // Gradually close the gap during animation
            CloseGapGradually();
            
            // Check if animation duration is over
            if (elapsedTime >= animationDuration)
            {
                // Force close the gap and end animation
                ForceCloseGap();
                isGapAnimationActive = false;
                Debug.Log("Gap animation ended: duration complete");
                return;
            }
            
            // Continue with current animation
            return;
        }
        
        // Find the largest gap in the chain
        float maxGap = 0f;
        int gapStart = -1;
        
        for (int i = 0; i < distances.Count - 1; i++)
        {
            if (yarns[i] != null && yarns[i + 1] != null)
            {
                float gap = distances[i + 1] - distances[i];
                if (gap > maxGap)
                {
                    maxGap = gap;
                    gapStart = i;
                }
            }
        }
        
        // Debug: Always log gap detection
        Debug.Log($"Checking gaps: maxGap={maxGap:F2}, threshold={yarnSpacing + gapThreshold:F2}, yarnSpacing={yarnSpacing:F2}");
        
        // Check if gap is large enough to trigger animation
        if (maxGap > yarnSpacing + gapThreshold)
        {
            // Start gap animation
            gapStartIndex = gapStart;
            gapEndIndex = gapStart + 1;
            isGapAnimationActive = true;
            animationStartTime = Time.time;
            originalGapSize = maxGap;
            Debug.Log($"🎯 GAP ANIMATION STARTED: gap size {maxGap:F2}, front at {gapStart}, back at {gapStart + 1}");
        }
    }
    
    private void ForceCloseGap()
    {
        if (gapStartIndex < 0 || gapEndIndex >= distances.Count)
            return;
            
        // Calculate how much to close the gap
        float gapSize = distances[gapEndIndex] - distances[gapStartIndex];
        float targetGap = yarnSpacing;
        float closeAmount = gapSize - targetGap;
        
        if (closeAmount > 0.001f)
        {
            // Move the back part forward to close the gap
            for (int i = gapEndIndex; i < distances.Count; i++)
            {
                if (yarns[i] != null)
                {
                    distances[i] -= closeAmount;
                }
            }
            
            Debug.Log($"Force closed gap: {gapSize:F2} -> {distances[gapEndIndex] - distances[gapStartIndex]:F2}");
        }
        
        // Triple-check: ensure perfect spacing
        float finalGap = distances[gapEndIndex] - distances[gapStartIndex];
        if (Mathf.Abs(finalGap - yarnSpacing) > 0.001f)
        {
            float exactClose = finalGap - yarnSpacing;
            for (int i = gapEndIndex; i < distances.Count; i++)
            {
                if (yarns[i] != null)
                {
                    distances[i] -= exactClose;
                }
            }
            Debug.Log($"Perfect gap closure: {finalGap:F2} -> {distances[gapEndIndex] - distances[gapStartIndex]:F2}");
        }
    }
    
    private void CloseGapGradually()
    {
        if (!isGapAnimationActive || gapStartIndex < 0 || gapEndIndex >= distances.Count)
            return;
            
        // Calculate current gap size
        float currentGap = distances[gapEndIndex] - distances[gapStartIndex];
        float targetGap = yarnSpacing;
        
        // If gap is already small enough, we're done
        if (currentGap <= targetGap + 0.01f)
        {
            // Force close to exact spacing
            float exactClose = currentGap - targetGap;
            if (exactClose > 0.001f)
            {
                for (int i = gapEndIndex; i < distances.Count; i++)
                {
                    if (yarns[i] != null)
                    {
                        distances[i] -= exactClose;
                    }
                }
            }
            
            isGapAnimationActive = false;
            Debug.Log("Gap animation ended: gap closed naturally");
            return;
        }
        
        // Calculate how much to close this frame (smooth closing)
        float gapReduction = (currentGap - targetGap) * Time.deltaTime * 8f; // Close 8x per second for very fast closing
        
        if (gapReduction > 0.001f)
        {
            // Move the back part forward to close the gap
            for (int i = gapEndIndex; i < distances.Count; i++)
            {
                if (yarns[i] != null)
                {
                    distances[i] -= gapReduction;
                }
            }
            
            Debug.Log($"Closing gap gradually: {currentGap:F2} -> {distances[gapEndIndex] - distances[gapStartIndex]:F2}");
        }
    }
    
    private float CalculateMovementSpeed(int index)
    {
        if (!isGapAnimationActive)
        {
            return speed; // Normal speed when no gap animation
        }
        
        float calculatedSpeed;
        
        // Front part (closer to end) moves slowly - wait for back to catch up
        if (index >= gapEndIndex)
        {
            calculatedSpeed = speed * frontSlowSpeed;
            if (index == gapEndIndex) // Only log for the first front yarn to avoid spam
            {
                Debug.Log($"🎯 FRONT PART (index {index}): speed = {speed} * {frontSlowSpeed} = {calculatedSpeed}");
            }
        }
        // Back part (further from end) moves faster to catch up to front
        else if (index <= gapStartIndex)
        {
            calculatedSpeed = speed * catchUpSpeed;
            if (index == gapStartIndex) // Only log for the first back yarn to avoid spam
            {
                Debug.Log($"🎯 BACK PART (index {index}): speed = {speed} * {catchUpSpeed} = {calculatedSpeed}");
            }
        }
        // Yarns in between move at normal speed
        else
        {
            calculatedSpeed = speed;
        }
        
        return calculatedSpeed;
    }
    
    private void TriggerGapAnimation()
    {
        // Reset gap animation state to check for new gaps
        isGapAnimationActive = false;
        gapStartIndex = -1;
        gapEndIndex = -1;
        animationStartTime = 0f;
        originalGapSize = 0f;
    }
}

[System.Serializable]
public class YarnPattern
{
    [Header("Pattern Settings")]
    public string patternName = "New Pattern";
    public string[] yarnColors = {"Red", "Green", "Blue", "Red", "Green", "Blue"};
    
    [Header("Pattern Description")]
    [TextArea(3, 5)]
    public string description = "Describe this pattern's difficulty and strategy";
    
    [Header("Pattern Notes")]
    public bool hasMatches = false;
    public int difficulty = 1; // 1 = Easy, 2 = Medium, 3 = Hard
}
