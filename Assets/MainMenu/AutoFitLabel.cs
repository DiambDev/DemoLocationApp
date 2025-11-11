using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
public class AutoFitButtons : MonoBehaviour
{
    public UIDocument ui;
    public float velocity = 0.1f;
    private Label movingLabel;

    private string fullText = "LLAMADA DE EMERGENCIA";
    private int index = 0;
    private int direction = 1; // 1 = hacia adelante, -1 = hacia atrás
    private int visibleChars = 12; // cuántos caracteres se muestran

    void Start()
    {
        var root = ui.rootVisualElement;

        movingLabel = root.Q<Label>("lblEmergencia");

        StartCoroutine(MoveText());
    }

    IEnumerator MoveText()
    {
        while (true)
        {
            // Calculamos inicio del substring
            int start = Mathf.Clamp(index, 0, fullText.Length - visibleChars);

            // Hacemos "ventana" del texto
            string part = fullText.Substring(start, visibleChars);

            movingLabel.text = part;

            // Avanzamos
            index += direction;

            // Revertimos si llegamos al fondo
            if (index >= fullText.Length - visibleChars || index <= 0)
                direction *= -1;

            yield return new WaitForSeconds(velocity); // velocidad
        }
    }
}
