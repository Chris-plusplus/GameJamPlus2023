using Interactables;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class SeedBag : Liftable
{
    [SerializeField] private SeedSO seedType;
    [SerializeField] private int seedCount;

    private Interactable interactable;
    private IInteracter interacter;

    protected override void Awake()
    {
        base.Awake();
        interactable = GetComponent<Interactable>();
    }

    protected override void OnInteractionChanged(bool isInteracting)
    {
        if (isInteracting)
        {
            if (!TryPlant())
            {
                liftedHolder.DropObject(this);
            }
            else if (seedCount <= 0)
            {
                liftedHolder.DropObject(this);
                Destroy(gameObject);
            }
        }
    }
    public override void PickUp(ILiftableHolder holder)
    {
        base.PickUp(holder);
        interacter = interactable.Interacter;
    }
    private bool TryPlant()
    {
        var target = interacter.SelectedObject;
        if (target != null && target.TryGetComponent(out FieldPatch fieldPatch))
        {
            Seed seed = Instantiate(seedType.seed, transform.position, transform.rotation);
            seed.PlantAt(fieldPatch);
            --seedCount;
            return true;
        }
        return false;
    }
}
