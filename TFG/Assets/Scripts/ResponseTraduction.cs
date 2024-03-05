using OpenAI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TraductionLogic : MonoBehaviour
{
    // Desde ChatGPT llamamos a esta funci�n para traducir el mensaje de respuesta. 
    // Desde esta funci�n llamamos a lo que haga falta para ejecutarla. Movimiento...

    public GameObject player;
    public GameObject enemy;
    public GameObject porton;

    public GameObject comic;
    public GameObject paraguas;
    public GameObject tronco;
    public GameObject mesa;
    public GameObject sofa;
    public GameObject silla;
    public GameObject vela;

    public GameObject teleportVFX;

    private OnClick onClick = new();

    private Animator naeveAnimator;

    // Tags del collider de cada gameobject de la escena
    //private const string comicTag = "c�mic";
    //private const string umbrellaTag = "paraguas";
    //private const string groundTag = "ground";
    //private const string playerTag = "player";
    //private const string troncoTag = "tronco";
    //private const string mesaTag = "mesa";
    //private const string sofaTag = "sof�";
    //private const string velaTag = "vela";
    //private const string sillaTag = "silla";
    //private const string aliadoTag = "aliado";

    private string actionText;
    private string naeveText;

    private string reply = "";
    private Vector2 target;
    private float posX, posY;
    private bool moving;
    private bool enemyAwake = false;

    private bool invisible = false;
    private float fadeSpeed = 1.0f;
    private float invisibleValue = 0.021f;

    private GameObject objectMoving = null;
    private GameObject objectInvisible = null;

    private int groundLayerMask;

    private const float speed = 10f;
    private float jumpForce = 700f;
    private bool isJumping = false;
    private const float interactionRange = 12f; // Rango de interacci�n con los objetos

    // Abecedario del lenguaje formal, dividido en acciones, objetos y entidades
    private string[] actions = { "coger", "mover", "transformar", "vibrar", "desaparecer", "menguar", "crecer", "explotar", "atacar", "esconderse", "atraer", "teletransportar", "soltar", "levitar", "materializar", "utilizar", "saltar", "hablar", "esperar", "caer", "invisibilizar" };
    private string[] objects = { "paraguas", "c�mic", "tronco", "sof�", "mesa", "vela", "silla" };
    private string[] entities = { "Naeve", "enemigo", "aliado" };
    private string[] objectsNoScene = { "llave", "puerta", "linterna" };
    //private GameObject[] gameObjectList = { player, comic, paraguas };
    private List<string> inventory = new();
    // Diccionario con la correspondencia string-gameobject para cada objeto y ente
    private Dictionary<string, GameObject> objectDictionary = new Dictionary<string, GameObject>();
    // Diccionario de string, correspondiente a la acci�n, y el m�todo que la implementa
    private Dictionary<string, Action<GameObject>> actionObjectLogic = new();

    //private void Awake()
    //{
    //    onClick = GameObject.FindObjectOfType<OnClick>();
    //}

    private void Start()
    {
        InitializeActionObjectLogic();
        InitializeGameObjectDictionary();
        // Obtenemos el componente animator
        naeveAnimator = player.GetComponent<Animator>();
        // Obtenemos la capa del suelo para poder saber cuando est� encima Naeve y cuando no
        groundLayerMask = LayerMask.GetMask("suelo");
    }

    private void Update()
    {
        if (enemy.activeSelf && !enemyAwake) // Comprueba si el enemigo ha aparecido ya para generar el prompt correspondiente.
        {
            enemyAwake = true;
            buildEnemyPrompt(); // Llamamos a un funci�n auxiliar para generar el mensaje y envi�rselo a GPT.

        }
        if (Input.GetMouseButtonDown(0) && !moving)
        {
            RaycastHit2D hit = onClick.GetPositionRay(Input.mousePosition);

            if (hit.collider != null/* && hit.collider.CompareTag(tag)*/)
            {
                string tag = hit.collider.tag;
                buildInteractionMsg(hit); // Llamamos a un funci�n auxiliar para generar el mensaje y envi�rselo a GPT.
            }
            else // No ha detectado ning�n collider, click en el escenario
            {
                // Podr�a poner un collider is trigger en el escenario y detecatarlo como el resto de cosas.
                Debug.Log("Click en el escenario en la posici�n: " + hit.point);
                //StartCoroutine(SendAndHandleReply("Click en el escenario en la posici�n: " + hit.point));
                buildOtherMsg("Click en el escenario en la posici�n: " + hit.point);
            }
        }

        if (moving && !objectMoving.Equals(null))
        {
            MoveTowardsTarget(objectMoving);
        }

        if (invisible && !objectInvisible.Equals(null))
        {
            InvisibleTarget(objectInvisible);
        }

        if (isJumping)
        {
            // Introduzco un peque�o retraso para que le de tiempo a comenzar a saltar antes de comprobar si Naeve no est� en el suelo. Ya que, de otro modo, se desactiva inmediatamente la animaci�n
            Invoke("CheckGroundedAfterJump", 0.1f);
        }
    }

    // Funci�n para checkear si Naeve ha tocado ya el suelo y podemos quitar la animaci�n de saltar
    private void CheckGroundedAfterJump()
    {
        if (IsGrounded())
        {
            naeveAnimator.SetBool("isJump", false);
            isJumping = false;
        }
    }

    // Construye el prompt cuando aparece el hombre del sombrero
    private async void buildEnemyPrompt()
    {
        string msg = "El enemigo (hombre del sombrero) ha aparecido en la escena y va hacia ti. Parece amenazador. Naeve tiene mucho miedo. Recuerda tus posibles acciones: Acci�n: {coger, mover, transformar, vibrar, desaparecer, menguar, crecer, explotar, atacar, esconderse, atraer, teletransportar, soltar, levitar, materializar, utilizar, saltar, hablar, esperar, caer, invisibilizar} y s�lo puedes utilizar tus objetos en el inventario, que son: ";

        if (inventory.Count == 0)
        {
            msg += "Inventario vac�o";
        }
        else
        {
            foreach (string item in inventory)
            {
                msg += item;
            }
        }
        Debug.Log(msg);
        await SendAndHandleReply(msg);
    }

    // Creamos el mensaje cuando hace click el jugador, y la corutina para enviarlo a GPT
    private async void buildInteractionMsg(RaycastHit2D hit)
    {
        string sendMsg = "El jugador ha hecho click en " + hit.collider.gameObject.name + ", en la posici�n: " + hit.point;
        await SendAndHandleReply(sendMsg);
    }

    //Creamos un mensaje cualquiera pasado por par�metro
    private async void buildOtherMsg(string msg)
    {
        await SendAndHandleReply(msg);
    }

    // Funci�n para mover a Naeve a un objetivo determinado
    private void MoveTowardsTarget(GameObject obj)
    {
        float step = speed * Time.deltaTime;
        obj.transform.position = Vector2.MoveTowards(obj.transform.position, target, step);

        if ((Vector2)objectMoving.transform.position == target)
        {
            moving = false;
            objectMoving = null;
            if (obj == player)
            {
                naeveAnimator.SetBool("isRun", false);
            }
        }
    }

    private void TeleportTowardsTarget(GameObject obj)
    {
        obj.transform.position = target;
    }
    // Vuelve invisible al objeto paulatinamente
    private void InvisibleTarget(GameObject obj)
    {
        // Obtenemos el renderer del objeto
        Renderer objectRenderer = obj.GetComponent<Renderer>();
        // Lerp (interpolaci�n lineal) entre la transparencia actual y 0 (totalmente transparente)
        float currentTransparency = objectRenderer.material.color.a;
        float newTransparency = Mathf.Lerp(currentTransparency, 0.02f, fadeSpeed * Time.deltaTime);
        // Actualiza el color del material con la nueva transparencia
        Color newColor = objectRenderer.material.color;
        newColor.a = newTransparency;
        objectRenderer.material.color = newColor;

        if (newTransparency <= invisibleValue)
        {
            invisible = false;
            objectInvisible = null;
        }
    }
    private void Jump(Rigidbody2D rb)
    {
        if (IsGrounded() & !naeveAnimator.GetBool("isJump")) // Asegura que el jugador est� en el suelo antes de saltar y que no est� la animaci�n ya activada
        {
            isJumping = true;
            naeveAnimator.SetBool("isJump", true);
            rb.velocity = new Vector2(rb.velocity.x, 0f); // Reinicia la velocidad en el eje Y
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private bool IsGrounded()
    {
        // Implementa la l�gica para verificar si el jugador est� en el suelo
        float raycastDistance = 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.down, raycastDistance, groundLayerMask);

        return hit.collider != null;
    }

    private async Task SendAndHandleReply(string msgSend)
    {
        reply = await GPTController.Instance.SendReply(msgSend);
        //Debug.Log("get answer 1: " + reply);
        while (string.IsNullOrEmpty(reply)) // Esperar hasta que la respuesta no est� vac�a
        {
            await Task.Delay(10);
        }
        // G
        // Parseamos el mensaje y obtenemos por un lado la acci�n y por el otro el texto en lenguaje natural
        getAction(reply, out actionText, out naeveText);

        GPTController.Instance.AppendMessage(naeveText);

        // Llama a traduce para traducir el mensaje a una acci�n.
        InterpretString(actionText);
        GPTController.Instance.UpdateGPT();
        // Resetea reply para futuras solicitudes
        reply = "";
    }

    //Este m�todo se llama desde GPTController cuando la respuesta est� lista
    public void SetReply(string response)
    {
        reply = response;
    }

    private void InitializeGameObjectDictionary()
    {
        objectDictionary.Add("paraguas", paraguas);
        objectDictionary.Add("c�mic", comic);
        objectDictionary.Add("tronco", tronco);
        objectDictionary.Add("sof�", sofa);
        objectDictionary.Add("mesa", mesa);
        objectDictionary.Add("vela", vela);
        objectDictionary.Add("silla", silla);
        objectDictionary.Add("Naeve", player);
        objectDictionary.Add("enemigo", enemy);
    }

    private void InitializeActionObjectLogic()
    {
        // PENSAR SOBRE SI PEDIR ALG�N PAR�METRO M�S A LA HORA DE MENGUAR, CRECER, TRANSFORMAR, LEVITAR...
        // Asignaci�n de l�gica a combinaciones espec�ficas con m�todos que aceptan GameObject como argumento
        actionObjectLogic.Add("mover", Mover); // HECHO
        actionObjectLogic.Add("coger", Coger); // HECHO
        actionObjectLogic.Add("transformar", Transformar);
        actionObjectLogic.Add("vibrar", Vibrar);
        actionObjectLogic.Add("desaparecer", Desaparecer); // HECHO
        actionObjectLogic.Add("menguar", Menguar); // HECHO
        actionObjectLogic.Add("crecer", Crecer); // HECHO
        actionObjectLogic.Add("explotar", Explotar);
        actionObjectLogic.Add("atacar", Atacar);
        actionObjectLogic.Add("esconder", Esconder);
        actionObjectLogic.Add("atraer", Atraer); // HECHO
        actionObjectLogic.Add("teletransportar", Teletransportar); // HECHO
        actionObjectLogic.Add("soltar", Soltar);
        actionObjectLogic.Add("levitar", Levitar); // HECHO
        actionObjectLogic.Add("materializar", Materializar); // HECHO
        actionObjectLogic.Add("utilizar", Utilizar);
        actionObjectLogic.Add("saltar", Saltar); // HECHO
        actionObjectLogic.Add("hablar", Hablar);
        actionObjectLogic.Add("esperar", Esperar); // HECHO
        actionObjectLogic.Add("caer", Caer);
        actionObjectLogic.Add("invisibilizar", Invisibilizar); // HECHO
    }

    void ExecuteActionObjectLogic(string action, GameObject obj)
    {
        string key = action;
        if (actionObjectLogic.ContainsKey(key))
        {
            actionObjectLogic[key].Invoke(obj);
        }
        else
        {
            Debug.Log("No se encontr� l�gica para la acci�n: " + key);
        }
    }

    private void Invisibilizar(GameObject obj)
    {
        if (!invisible)
        {
            invisible = true;
            objectInvisible = obj;
            InvisibleTarget(obj);
        }
    }

    // Si el objeto tiene rigidbody que est� desactivado, lo activa. De esta forma el objeto caer�
    private void Caer(GameObject obj)
    {
        if (obj.GetComponent<Rigidbody2D>().IsSleeping())
        {
            obj.GetComponent<Rigidbody2D>().WakeUp();
        }
    }

    private void Esperar(GameObject obj)
    {
        Debug.Log("Naeve est� esperando");
    }

    private void Hablar(GameObject obj)
    {
        Debug.Log("L�gica para hablar con el ente: " + obj.name);
        if (isOnRange(obj))
        {
            // Llamar a la clase para hablar con NPCs
        }
        else
        {
            //string sendMsg = "Est�s muy lejos del objeto. Mu�vete cerca del objeto antes";
            //StartCoroutine(SendAndHandleReply(sendMsg));
            Debug.Log("Acercate antes");
        }
    }

    private void Saltar(GameObject obj)
    {
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        Jump(rb);
    }

    // M�todos con l�gica para acciones espec�ficas que aceptan un GameObject como argumento
    private void Utilizar(GameObject obj)
    {
        throw new NotImplementedException();
    }

    private void Materializar(GameObject obj)
    {
        // Instancio el prefab al lado de Naeve
        Vector3 spawnPos = player.transform.position + 5 * (Vector3.right + Vector3.up);
        Instantiate(obj, spawnPos, Quaternion.identity);
    }

    private void Levitar(GameObject obj)
    {
        Debug.Log("L�gica para hacer levitar el objeto: " + obj.name);
        // Hacemos que el objeto dado se mueva a la posici�n posx, posy, dada por chatGPT y guardada en la funci�n anterior
        target = obj.transform.position; // El target pasa a ser un punto por encima del objeto
        target.y = obj.transform.position.y + 10f;
        moving = true;
        objectMoving = obj;
        MoveTowardsTarget(obj);
    }

    // Suelta el objeto en el mismo lugar en el que se cogi�, ya que s�lo lo desactivamos y lo activamos.
    private void Soltar(GameObject obj)
    {
        if (inventory.Contains(obj.tag))
        {
            inventory.Remove(obj.tag);
            // Al hacer esto, deja el objeto en el mismo sitio en el que estaba
            obj.SetActive(true);
            //Instantiate(obj, transform.position, transform.rotation);
        }
        else
        {
            Debug.Log("El objeto que quieres soltar no est� en el inventario");
        }

    }

    // Teletransporta a Naeve al objeto obj
    private void Teletransportar(GameObject obj)
    {
        Debug.Log("L�gica para teletransportar el objeto: " + obj.name);
        // Hacemos que el objeto dado se mueva a la posici�n posx, posy, dada por chatGPT y guardada en la funci�n anterior
        // Si la altura es menor que el suelo, teletransportamos al suelo.
        //posY = (posY < 2.2525f) ? 2.2525f : posY;
        if (posY < 2.2525f) posY = 2.2525f;
        target = new Vector2(posX, posY); // Creamos el vector 2D con las posiciones del objeto
        // A�ado Vextor3.up * 5.0f para que el efecto sea en el cuerpo de Naeve y no en los pies
        Instantiate(teleportVFX, obj.transform.position + Vector3.up * 4.0f, Quaternion.identity);
        TeleportTowardsTarget(obj);
        Instantiate(teleportVFX, target, Quaternion.identity);
    }

    private void Atraer(GameObject obj)
    {
        Debug.Log("L�gica para atraer el objeto: " + obj.name);
        // Hacemos que el objeto dado se mueva a la posici�n posx, posy, dada por chatGPT y guardada en la funci�n anterior
        target = (Vector2)player.transform.position; // El target en este caso ser�a la propia Naeve
        moving = true;
        objectMoving = obj;
        MoveTowardsTarget(obj);
    }

    private void Esconder(GameObject obj)
    {
        throw new NotImplementedException();
    }

    private void Atacar(GameObject obj)
    {
        throw new NotImplementedException();
    }

    private void Explotar(GameObject obj)
    {
        throw new NotImplementedException();
    }

    // Quiz� en la funci�n crecer, pedir a chat gpt cuanto tiene que crecer.
    private void Crecer(GameObject obj)
    {
        float newX = obj.transform.localScale.x * 2;
        float newY = obj.transform.localScale.y * 2;
        StartCoroutine(CambiarTama�oConTransicion(obj, newX, newY));
    }

    private void Menguar(GameObject obj)
    {
        float newX = obj.transform.localScale.x / 2;
        float newY = obj.transform.localScale.y / 2;
        StartCoroutine(CambiarTama�oConTransicion(obj, newX, newY));
    }

    private void Desaparecer(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void Vibrar(GameObject obj)
    {
        throw new NotImplementedException();
    }

    private void Transformar(GameObject obj)
    {
        throw new NotImplementedException();
    }

    void Mover(GameObject obj)
    {
        Debug.Log("L�gica para mover el objeto: " + obj.name);
        // Hacemos que el objeto dado se mueva a la posici�n posx, posy, dada por chatGPT y guardada en la funci�n anterior
        target = new Vector2(posX, posY); // Creamos el vector 2D con las posiciones del objeto
        posX = 0; posY = 0; // * Reseteo posiciones para depuraci�n
        moving = true;
        objectMoving = obj;
        if (obj == player && IsGrounded()) // Si el objeto dado es el jugador s�lo se puede mover en el eje x, adem�s activamos la animaci�n de correr
        {
            target.y = player.transform.position.y;
            // Si el target est� a la izquierda, Naeve correr�a hacia la izquierda. Si no, hacia la derecha. Utilizo la posici�n x del vector en negativo o positivo para hacer esto
            // Adem�s, como la escala de Naeve es de 2.2, adem�s del - o + en la direcci�n, mantenemos la escala de 2.2
            float direction;
            if (target.x < player.transform.position.x)
            {
                direction = -2.2f;
                player.transform.localScale = new Vector3(direction, 2.2f, 1);
            }
            else
            {
                direction = 2.2f;
                player.transform.localScale = new Vector3(direction, 2.2f, 1);
            }
            naeveAnimator.SetBool("isRun", true);
        }
        if (IsGrounded()) MoveTowardsTarget(obj);
    }

    private void Coger(GameObject obj)
    {
        Debug.Log("L�gica para coger el objeto: " + obj.name);
        if (isOnRange(obj))
        {
            inventory.Add(obj.tag); // A�adimos el objeto al inventario
            obj.SetActive(false);
            //Destroy(obj);
            Debug.Log("A�adido: " + inventory[inventory.Count - 1] + " al inventario.");
        }
        else
        {
            string sendMsg = "Est�s muy lejos del objeto. Mu�vete cerca del objeto antes";
            buildOtherMsg(sendMsg);
            Debug.Log("Acercate al objeto antes");
        }
    }

    // Utilizo una corutina para ir menguando y creciendo poco a poco el objeto
    IEnumerator CambiarTama�oConTransicion(GameObject obj, float newX, float newY)
    {
        float timeSpent = 0f;
        float transitionTime = 2f;

        Vector3 iniSize = obj.transform.localScale;
        Vector3 newSize = new Vector3(newX, newY, obj.transform.localScale.z);

        while (timeSpent < transitionTime)
        {
            timeSpent += Time.deltaTime;

            // Interpola suavemente entre el tama�o inicial y el nuevo tama�o
            obj.transform.localScale = Vector3.Lerp(iniSize, newSize, timeSpent / transitionTime);

            yield return null;
        }

        // Asegura que el tama�o final sea el que queremos
        obj.transform.localScale = newSize;
    }

    private bool isOnRange(GameObject obj)
    {
        float distance = Vector2.Distance(player.transform.position, obj.transform.position);
        if (distance <= interactionRange) return true;
        return false;
    }

    // Separamos el texto que es de acciones de Naeve y el hablado. Utilizo par�metros de salida para devolver ambos textos.
    public void getAction(string message, out string actionText, out string naeveText)
    {
        actionText = "";
        naeveText = message;

        int indiceCorcheteApertura = -1;
        int indiceCorcheteCierre = -1;

        while (true)
        {
            indiceCorcheteApertura = naeveText.IndexOf('[', indiceCorcheteApertura + 1);
            indiceCorcheteCierre = naeveText.IndexOf(']', indiceCorcheteCierre + 1);

            if (indiceCorcheteApertura != -1 && indiceCorcheteCierre != -1 && indiceCorcheteCierre > indiceCorcheteApertura)
            {
                string textoEntreCorchetes = naeveText.Substring(indiceCorcheteApertura + 1, indiceCorcheteCierre - indiceCorcheteApertura - 1);
                string textoAntesDeCorchetes = naeveText.Substring(0, indiceCorcheteApertura);
                string textoDespuesDeCorchetes = naeveText.Substring(indiceCorcheteCierre + 1).Trim();

                //Debug.Log("Texto entre corchetes: " + textoEntreCorchetes);
                //Debug.Log("Texto antes de corchetes: " + textoAntesDeCorchetes);
                //Debug.Log("Texto despu�s de corchetes: " + textoDespuesDeCorchetes);

                // CUIDADO POR SI ELIMINO ALGO QUE NO TOCA
                naeveText = textoAntesDeCorchetes.TrimEnd(',', ' ') + textoDespuesDeCorchetes.TrimEnd(',', ' ');
                actionText = actionText + "/" + textoEntreCorchetes;

                indiceCorcheteApertura = -1;
                indiceCorcheteCierre = -1;
            }
            else
            {
                break;
            }
        }

        Debug.Log("Texto restante final: " + naeveText);
        Debug.Log("Acciones: " + actionText);
    }

    public void InterpretString(string inputString)
    {
        Debug.Log("Has entrado en el interprete");
        string[] parts = inputString.Split('/');

        if (parts.Length >= 3)
        {
            string action = parts[1];
            string[] subParts;

            // Si el formato es /accion/objeto, si hay un comando extra para marcar la posici�n /posx,posy, estar� en la posici�n 3, si el formato es /accion, estar� en la 2
            if (parts.Length == 4)
            {
                subParts = parts[3].Split(',');
            }
            else
            {
                subParts = parts[2].Split(',');
            }

            if (parts.Length == 3 && subParts.Length == 1) // Formato: /accion/objeto
            {
                string objeto = parts[2];
                Debug.Log("Acci�n: " + action);
                Debug.Log("Objeto: " + objeto);

                GameObject objectParsed;

                if (action == actions[14]) // Si la acci�n es materializar, la funci�n es un poco diferente, por lo que lo trato como un caso aparte
                {
                    Debug.Log("Estamos dentro de la acci�n");
                    objectParsed = GetPrefabToSpawn(objeto);
                    if (objectParsed != null)
                    {
                        ExecuteActionObjectLogic(action, objectParsed);
                    }
                }
                else if (objectDictionary.TryGetValue(objeto, out objectParsed))
                {
                    ExecuteActionObjectLogic(action, objectParsed);
                }
                else
                {
                    Debug.Log("Error: No se ha encontrado el objeto");
                }
            }
            else if (parts.Length == 3 && subParts.Length == 2) // Formato: /accion/posicionx,posiciony
            {
                // Parseamos las coordenadas si es posible, y las guardamos en los atributos de posicion
                // Dado que el formato que le mando a hactgpt es 8.9 en vez de 8,9, tengo que a�adir NumberStyles.Float, CultureInfo.InvariantCulture
                // Puedo cambiar el formato pedido y ahorrarme esto
                if (float.TryParse(subParts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out posX) && float.TryParse(subParts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out posY))
                {
                    Debug.Log("Acci�n: " + action);
                    Debug.Log("Posici�n X: " + posX);
                    Debug.Log("Posici�n Y: " + posY);
                    // Confirmamos que la accion sea mover
                    if (action == actions[1])
                    {
                        // En este caso, tenemos s�lo una orden de mover y la posici�n, por lo que hay que mover a Naeve(player)
                        ExecuteActionObjectLogic(action, player);
                    }
                }
                else
                {
                    Debug.LogError("Formato de posici�n no v�lido.");
                }
            }
            else if (parts.Length == 4 && subParts.Length == 1) // Formato: /accion/objeto/objeto
            {
                string objeto1 = parts[2];
                string objeto2 = parts[3];
                Debug.Log("Acci�n: " + action);
                Debug.Log("Objeto 1: " + objeto1);
                Debug.Log("Objeto 2: " + objeto2);
                // Por ahora no he conseguido implementar esto por tener dos argumentos la funci�n (objeto1, objeto2)
            }
            else if (parts.Length == 4 && subParts.Length == 2) // Formato: /accion/objeto/posicionx,posiciony
            {
                string objeto = parts[2];
                if (float.TryParse(subParts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out posX) && float.TryParse(subParts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out posY))
                {
                    Debug.Log("Acci�n: " + action);
                    Debug.Log("Objeto: " + objeto);
                    Debug.Log("Posici�n X: " + posX);
                    Debug.Log("Posici�n Y: " + posY);

                    GameObject objectParsed;
                    // Si la acci�n es teletransportar y el objeto est� en el diccionario de objetos y entes
                    if (/*action == actions[11] && */objectDictionary.TryGetValue(objeto, out objectParsed))
                    {
                        // En este caso, tenemos s�lo una orden de teletransportar, el objeto o ente y la posici�n.
                        ExecuteActionObjectLogic(action, objectParsed);
                    }
                }
                else
                {
                    Debug.LogError("Formato de posici�n no v�lido.");
                }
            }
            else
            {
                Debug.LogError("Formato no reconocido.");
            }
        }
        else if (parts.Length == 2)
        {
            string action = parts[1];
            // Nos aseguramos de que la acci�n sea \esperar o \saltar
            Debug.Log("En este punto entramos en el saltador");
            ExecuteActionObjectLogic(action, player);
            //if (action == actions[18] || action == actions[16])
            //{
            //    ExecuteActionObjectLogic(action, player);
            //}
        }
        else
        {
            Debug.LogError("Formato no v�lido.");
        }
    }

    private GameObject GetPrefabToSpawn(string objeto)
    {
        // Me aseguro de que el formato del nombre es el correcto
        string prefabName = char.ToUpper(objeto[0]) + objeto.Substring(1).ToLower();
        // Intento cargar el prefab desde la carpeta
        Debug.Log("El nombre del prefab a buscar ser�: " + prefabName);
        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + prefabName);
        Debug.Log("El prefab encontrado es:" + prefab.name);
        if (prefab != null)
        {
            // Si se encontr� el prefab, lo devuelve
            return prefab;
        }
        else
        {
            // Si no se encontr� el prefab, muestra una advertencia
            Debug.LogWarning("Prefab no encontrado con el nombre: " + prefabName);
            return null;
        }

    }
}
