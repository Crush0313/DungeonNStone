using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public Item item; //해당 아이템, 외부에서 지정해줌

    private GameObject _player; //플레이어
    private Inventory _inventory; //메인 인벤토리
    // Use this for initialization

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player != null)
            _inventory = _player.GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        //인벤토리가 있고, E키를 누르면
        if (_inventory != null && Input.GetKeyDown(KeyCode.E))
        {
            //근처일 때
            float distance = Vector3.Distance(this.gameObject.transform.position, _player.transform.position);
            if (distance <= 3)
            {
                bool check = _inventory.checkIfItemAllreadyExist(item.itemID, item.itemValue);

                if (check)
                    Destroy(this.gameObject);

                else if (_inventory.ItemsInInventory.Count < (_inventory.width * _inventory.height))
                {
                    _inventory.addItemToInventory(item.itemID, item.itemValue);
                    _inventory.updateItemList();
                    _inventory.stackableSettings();
                    Destroy(this.gameObject);
                }
            }
        }
    }

}