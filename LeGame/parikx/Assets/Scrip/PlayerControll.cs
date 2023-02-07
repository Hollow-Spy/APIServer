using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    Vector2 _Input;
    [SerializeField] CharacterController p_controller;
    [SerializeField] float Speed;
    // Update is called once per frame
    void Update()
    {
        _Input.x = Input.GetAxisRaw("Horizontal");
        _Input.y = Input.GetAxisRaw("Vertical");

        p_controller.Move(_Input * Speed * Time.deltaTime);
    }
}
