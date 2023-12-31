﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenGraphics : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;

    void Update()
    {
        transform.Rotate(transform.forward * rotationSpeed * Time.deltaTime);
    }
}
