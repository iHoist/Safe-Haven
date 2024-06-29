using UnityEngine;

public class PlayerHandler : MonoBehaviour {
    public static PlayerHandler instance;
    [HideInInspector] public PlayerInputActions inputActions;
    private Animator animator;

    [Header("Movement Variables")]
    public float speed = 4;
    private Vector3 direction;

    [Header("Inventory Variables")]
    public int inventorySize = 21;
    public Inventory inventory;

    [Header("Action Variables")]
    private bool isInteracting;
    private float chopCooldown = 0;

    private void Awake() {
        instance = this;
        inputActions = new PlayerInputActions();
        inventory = new Inventory(inventorySize);
    }
    private void Start() {
        animator = GetComponentInChildren<Animator>();
        inputActions.Enable();
    }
    private void Update() {
        if (inputActions.Player.ToggleMenu.WasPressedThisFrame()) CraftingManager.instance.ToggleCraftingMenu();

        if (!CraftingManager.instance.menuOn) {
            direction = inputActions.Player.Move.ReadValue<Vector2>(); AnimateMvoement(direction);
            isInteracting = !(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Run"));
            UpdateActionVariables();

            if (inputActions.Player.DropItem.WasPressedThisFrame()) DropItem();
            if (inputActions.Player.Interact.WasPressedThisFrame()) Interact();
            if (inputActions.Player.Place.WasPressedThisFrame()) Place();
        } else {
            direction = new(); AnimateMvoement(direction);
        }
    }
    private void FixedUpdate() {
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }

    private void UpdateActionVariables() {
        if (chopCooldown > 0) chopCooldown -= Time.deltaTime; if (chopCooldown < 0) chopCooldown = 0;
    }

    private void AnimateMvoement(Vector3 direction) {
        if (animator != null) {
            animator.SetBool("isMoving", direction.magnitude > 0);
            if (direction.magnitude > 0) {
                animator.SetFloat("horizontal", direction.x);
                animator.SetFloat("vertical", direction.y);
            }
        }
    }

    private void DropItem() {
        int index = InventoryUI.instance.selectedSlot;
        if (index >= 0) {
            GameObject itemToDrop = InventoryUI.instance.SelectedInventoryObject();

            Vector3 spawnLocation = transform.position;
            Vector3 spawnOffset = Random.insideUnitCircle * 1.25f;

            if (itemToDrop != null) {
                GameObject droppedItem = Instantiate(itemToDrop, spawnLocation + spawnOffset, Quaternion.identity); inventory.RemoveItem(index);
                droppedItem.GetComponent<Rigidbody2D>().AddForce(spawnOffset * 2f, ForceMode2D.Impulse);
            }
        }
    }

    private void Interact() {
        GameObject selectedObject = GetComponentInChildren<Selector>().selectedObject;

        //Chop Tress
        if (IsHoldingItemType("Axe") && selectedObject != null && selectedObject.GetComponent<TreeObject>() != null && !isInteracting && chopCooldown <= 0) {
            selectedObject.GetComponent<TreeObject>().DamageTree(20);
            if (animator != null) animator.SetTrigger("isChopping");
            chopCooldown += 0.5f;
        }
        //Fill Empty Bucket
        else if (IsHoldingItem("WoodBucket (Empty)") && IsFacingTile(WorldHandler.instance.groundGrid, GroundCell.Type.Water)) {
            inventory.RemoveItem(InventoryUI.instance.selectedSlot);
            inventory.AddItem(AssetFinder.FindPrefabByObjectName("WoodBucket (Water)").GetComponent<Collectable>());
        }
        //Till Ground
        else if (IsHoldingItemType("Hoe") && IsStandingOnTile(WorldHandler.instance.groundGrid, GroundCell.Type.Land)) {
            Vector3 standingPos = new(transform.position.x, transform.position.y - 1f, transform.position.z);
            int cellPosX = WorldHandler.instance.WorldPosToCellPos(standingPos).x;
            int cellPosY = WorldHandler.instance.WorldPosToCellPos(standingPos).y;
            WorldHandler.instance.AddSoilPlot(cellPosX, cellPosY);
        }
        //Plant Seed
        else if (IsHoldingItemType("Seed") && IsStandingOnTile(WorldHandler.instance.soilGrid, SoilCell.Type.Soil) && IsStandingOnTile(WorldHandler.instance.plantGrid, PlantCell.Type.None)) {
            string spriteName = InventoryUI.instance.SelectedInventoryObject().GetComponent<Collectable>().itemSO.icon.name;
            Vector3 standingPos = new(transform.position.x, transform.position.y - 1f, transform.position.z);
            int cellPosX = WorldHandler.instance.WorldPosToCellPos(standingPos).x;
            int cellPosY = WorldHandler.instance.WorldPosToCellPos(standingPos).y;
            WorldHandler.instance.PlantSeedAtPos(cellPosX, cellPosY, spriteName);

            inventory.RemoveItem(InventoryUI.instance.selectedSlot);
        }
        //Harvest Plant
        else if (IsStandingOnTile(WorldHandler.instance.soilGrid, SoilCell.Type.Soil) && !IsStandingOnTile(WorldHandler.instance.plantGrid, PlantCell.Type.None)) {
            Vector3 standingPos = new(transform.position.x, transform.position.y - 1f, transform.position.z);
            int cellPosX = WorldHandler.instance.WorldPosToCellPos(standingPos).x;
            int cellPosY = WorldHandler.instance.WorldPosToCellPos(standingPos).y;
            WorldHandler.instance.HarvestPlantAtPos(cellPosX, cellPosY);
        }
    }

    private void Place() {
        int index = InventoryUI.instance.selectedSlot;
        if (index >= 0) {
            GameObject itemToPlace = InventoryUI.instance.SelectedInventoryObject();

            if (itemToPlace != null && !Cursor.instance.IsCursorIconNull()) {
                GameObject placedItem = Instantiate(itemToPlace, Cursor.instance.transform.position, Quaternion.identity); inventory.RemoveItem(index);
                placedItem.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
    }

    private bool IsStandingOnTile(Cell[,] grid, Cell.Type tileForDetection) {
        Vector3 detectorPos = new(transform.position.x, transform.position.y - 1f, 0f);
        Vector3Int cellPos = WorldHandler.instance.WorldPosToCellPos(detectorPos);
        return WorldHandler.instance.CheckCellAtPos(grid, cellPos.x, cellPos.y, tileForDetection);
    }

    private bool IsFacingTile(Cell[,] grid, Cell.Type tileForDetection) {
        Vector3 detectorPos = new(transform.position.x + (animator.GetFloat("horizontal") * 1.5f), transform.position.y + (animator.GetFloat("vertical") * 1.5f) - 1f, 0f);
        Vector3Int cellPos = WorldHandler.instance.WorldPosToCellPos(detectorPos);
        return WorldHandler.instance.CheckCellAtPos(grid, cellPos.x, cellPos.y, tileForDetection);
    }

    private bool IsHoldingItem(string item) { return InventoryUI.instance.SelectedInventoryObject() != null && InventoryUI.instance.SelectedInventoryObject().name.Equals(item); }
    private bool IsHoldingItemType(string itemType) { return InventoryUI.instance.SelectedInventoryObject() != null && InventoryUI.instance.SelectedInventoryObject().name.Contains(itemType); }
}