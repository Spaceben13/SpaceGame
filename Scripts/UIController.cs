using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public UIDocument uiDocument;
    public CivilizationSim sim;   // drag your CivilizationSim GameObject here

    private DoubleField gdpField;
    private DoubleField gdpRevField;

    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        // Query the DoubleField by name
        gdpField = root.Q<DoubleField>("GDP");
        gdpRevField = root.Q<DoubleField>("GDP_Revenue");
    }

    void Update()
    {
        // Update the UI every frame
        gdpField.value = sim.GDP;
        

    }
}



