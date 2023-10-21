using Interactables;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Growable : Interactable
{
    public FieldPatch fieldPatch;
    [SerializeField] private SeedSO seed;
    [SerializeField] private float time = 0;
    [SerializeField] public bool Grown => time >= seed.growInfo.time;

    private InteractionHighlightController outlineController;

    protected override void Awake()
    {
        base.Awake();
        outlineController = GetComponent<InteractionHighlightController>();
        outlineController.enabled = false;
        OnSelectionChanged += UpdateSelect;
    }

    public void Init(FieldPatch f)
    {
        fieldPatch = f;
        transform.position = fieldPatch.PlantPoint.position;
        transform.rotation = Quaternion.Euler(0, Random.value * 360.0f, 0);
        time = 0;
        StartCoroutine(Grow());
    }

    private void Harvest(bool input)
    {
        if (input)
        {
            fieldPatch.Destroy();
            Destroy(gameObject);
            // doda� do inventory czy co�
        }
    }
    
    private IEnumerator Grow()
    {
        while(time < seed.growInfo.time)
        {
            time += Time.deltaTime;
            float lerped = Mathf.Lerp(0.0f, seed.growInfo.time, time / seed.growInfo.time) / seed.growInfo.time;
            transform.localScale = lerped * seed.growInfo.scale * Vector3.one;
            if (time >= seed.growInfo.time)
            {
                time = seed.growInfo.time;
                transform.localScale = seed.growInfo.scale * Vector3.one;
            }
            yield return null;
        }
        UpdateSelect(Interacter?.SelectedObject == this);
    }
    private void UpdateSelect(bool isSelected)
    {
        if (isSelected && Grown && Interacter is ILiftableHolder liftableHolder && liftableHolder.HeldObject is null)
        {
            outlineController.UpdateOutline(true);
            OnInteractionChanged += Harvest;
            ShowPointerOnInterract = true;
        }
        else
        {
            outlineController.UpdateOutline(false);
            OnInteractionChanged -= Harvest;
            ShowPointerOnInterract = false;
        }
    }
}
