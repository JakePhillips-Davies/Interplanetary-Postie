using System;
using UnityEngine;
using UnityEngine.UIElements;

public class AddVelocity : MonoBehaviour
{
    [SerializeField] UIDocument ui;
    [SerializeField] private AutoCentre centre;
    [SerializeField] private double speedToAdd = 10;
    private Orbit orbit;
    private double speed;
    public String speedString;
    void Awake() {
        orbit = GetComponent<Orbit>();
        
        ui.rootVisualElement.Q<TextField>("Speed").dataSource = this;
        ui.rootVisualElement.Q<Button>("Sub").clickable.clicked += () => {
            AddSpeed(0 - speedToAdd);
        };
        ui.rootVisualElement.Q<Button>("Add").clickable.clicked += () => {
            AddSpeed(speedToAdd);
        };
    }

    void FixedUpdate()
    {
        orbit = centre.objectList[centre.focus].GetComponent<Orbit>();
        speed = orbit.getLocalVel().magnitude;
        speedString = "Speed: " + speed;
    }

    void AddSpeed(double speed) { 
        Vector3d direction = orbit.getLocalVel().normalized;

        orbit.set_linear_velocity(orbit.getLocalVel() + direction * speed);
    }
}
