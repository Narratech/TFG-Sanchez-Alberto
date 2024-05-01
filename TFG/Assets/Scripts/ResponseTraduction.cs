using OpenAI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TraductionLogic : MonoBehaviour
{
    // Desde ChatGPT llamamos a esta función para traducir el mensaje de respuesta. 
    // Desde esta función llamamos a lo que haga falta para ejecutarla. Movimiento...

    // Creamos las instancisa para utilizar todas las funciones lógicas
    private LogicController logicController = new LogicController();
    private PromptManager promptManager = new PromptManager();
    private OnClick onClick = new();
    private Utilizador utilizarController = new Utilizador();

    public GPTController naeveControllerGPT;
    public GPTController actionControllerGPT;
    public GPTController errorControllerGPT;
    //public GPTController resumenControllerGPT;
    public GPTController portonControllerGPT;
    public GPTController estatuaControllerGPT;
    public GPTController quimeraControllerGPT;
    public GPTController loboControllerGPT;
    public GPTController guardaControllerGPT;

    public GameObject canvasObj;

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
    public GameObject cofre;
    public GameObject estatua;
    public GameObject arbol;
    public GameObject quimera;
    public GameObject lobo;
    public GameObject guarda;

    public GameObject teleportVFX;
    public GameObject explosionVFX;
    public GameObject portalVFX;

    private Animator naeveAnimator;

    private string actionText;
    private string naeveText;

    // Hay que ignorar el primer mensaje de GPT
    //private bool firstMessage = true;

    private string reply = "";
    private string errorReply = "";
    private string chatReply = "";
    private string correction = "";
    private bool errorTrigger = false;

    private Vector2 target;
    private float posX, posY;
    private bool moving;
    private bool enemyAwake = false;

    private bool invisible = false;
    private bool isInvisible = false;
    // Utilizo tanto hidde como hidden para poder "desesconder" a Naeve en cuanto se mueva. Si no utilizase ambos bool en cuanto Naeve se mueva al esconderse, se quitaría el cambio de capa
    private bool hidde = false;
    private bool hidden = false;
    private float fadeSpeed = 3.0f;
    private float invisibleValue = 0.021f;

    private bool cofreActivado = false;
    private bool attacking = false;
    private GameObject objAttacked = null;

    private GameObject objectMoving = null;
    private GameObject objectInvisible = null;

    public GameObject deathMenuUI;

    private int groundLayerMask;

    private float speed = 10f;
    private float jumpForce = 700f;
    private bool isJumping = false;
    private const float interactionRange = 11f; // Rango de interacción con los objetos

    private bool paraguasActivated = false;

    // Abecedario del lenguaje formal, dividido en acciones, objetos y entidades
    private string[] actions = { "coger", "mover", "transformar", "vibrar", "desaparecer", "menguar", "crecer", "explotar", "atacar", "esconderse", "atraer", "teletransportar", "soltar", "levitar", "materializar", "utilizar", "saltar", "hablar", "esperar", "caer", "invisibilizar", "controlar" };
    private string[] objects = { "paraguas", "cómic", "tronco", "sofá", "mesa", "vela", "silla", "cofre", "árbol" };
    private string[] entities = { "Naeve", "enemigo", "portón", "estatua", "quimera", "lobo", "guarda" };
    private string[] objectsNoScene = { "llave", "puerta", "linterna" };
    //private GameObject[] gameObjectList = { player, comic, paraguas };
    private List<string> inventory = new();
    // Diccionario con la correspondencia string-gameobject para cada objeto y ente
    private Dictionary<string, GameObject> objectDictionary = new Dictionary<string, GameObject>();
    // Diccionario de string, correspondiente a la acción, y el método que la implementa
    private Dictionary<string, Action<GameObject>> actionObjectLogic = new();

    private void Start()
    {
        InitializeActionObjectLogic();
        InitializeGameObjectDictionary();
        // Obtenemos el componente animator
        naeveAnimator = player.GetComponent<Animator>();
        // Obtenemos la capa del suelo para poder saber cuando está encima Naeve y cuando no
        groundLayerMask = LayerMask.GetMask("suelo");
        logicController.SetBodyParts(player.transform);

        InitializePorton();
        InitializeEstatua();
        //InitializeQuimera();
        //InitializeLobo();
        //InitializeGuarda();

        //GenerateErrorGPT();
        // Añado esto porque he tenido problemas para que la partida no esté pausada al resetearse
        PauseMenu.gameIsPaused = false;

        //string message = "Lenguaje formal: /Mover/59.58,4.69\r\nLenguaje formal: /Empujar/portón";
        //cosa(message);
        //ProcesarComandos(message);

        //Esconderse(sofa);
        //Hablar(porton);
        //Levitar(player);
    }

    private void ProcesarComandos(string message)
    {
        // Ajuste en la expresión regular para capturar adecuadamente el patrón deseado.
        // Esta expresión captura '/Comando/parametros' seguido opcionalmente por otros caracteres,
        // pero solo nos interesan los grupos capturados antes del espacio o fin de la cadena.
        var match = Regex.Match(message, @"(\/\w+\/[^ \r\n]*)");

        // Ahora 'actionText' contendrá solo el texto que coincide con el patrón deseado.
        string actionText = match.Success ? match.Groups[1].Value : "";

        // Limpieza final para eliminar caracteres no deseados, si es necesario.
        // Esta línea puede ser opcional dependiendo de si tu expresión regular ya asegura el formato deseado.
        actionText = Regex.Replace(actionText, @"[^\w\/,.]", "");

        Debug.Log("TEXTO NUEVO: " + actionText);
    }


    private void cosa(string message)
    {
        var match = Regex.Match(message, @"\/(\w+)(\/[^ ]*)?");
        actionText = match.Value;

        int spaceIndex = actionText.IndexOf(' ');
        if (spaceIndex != -1)
        {
            // Si hay un espacio, cortar la cadena hasta ese punto
            actionText = actionText.Substring(0, spaceIndex);
        }

        actionText = Regex.Replace(actionText, @"[^\w\d/,.]", "");

        Debug.Log("TEXTO: " + actionText);
    }

    private void Update()
    {
        if (PauseMenu.gameIsPaused) // Asumiendo que isGamePaused es una variable estática o accesible globalmente
        {
            Debug.Log("JUEGO PAUSADO");
            return; // Ignora el resto del código en Update si el juego está pausado
        }
       
        if (enemy.activeSelf && !enemyAwake) // Comprueba si el enemigo ha aparecido ya para generar el prompt correspondiente.
        {
            enemyAwake = true;
            buildEnemyPrompt(); // Llamamos a un función auxiliar para generar el mensaje y enviárselo a GPT.
        }

        if (PlayerMovement.playerControl)
        {
            Debug.Log("JUEGO CONTROLADO");
            return; // Ignora el resto del código en Update si el juego está siendo controlado por el jugador
        }

        if (Input.GetMouseButtonDown(0) && !moving)
        {
            RaycastHit2D hit = onClick.GetPositionRay(Input.mousePosition);

            if (hit.collider != null)
            {
                Debug.Log("Collider detectado:" + hit.collider.tag);
                if (hit.collider.tag == "portal")
                {
                    EndGame();
                }
                buildInteractionMsg(hit); // Llamamos a un función auxiliar para generar el mensaje y enviárselo a GPT.
            }
            else // No ha detectado ningún collider, click en el escenario
            {
                // Podría poner un collider is trigger en el escenario y detecatarlo como el resto de cosas.
                Debug.Log("Click en el botón hablar.");
                //StartCoroutine(SendAndHandleReply("Click en el escenario en la posición: " + hit.point));
                //buildOtherMsg("Click en el escenario.");
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
            // Introduzco un pequeño retraso para que le de tiempo a comenzar a saltar antes de comprobar si Naeve no está en el suelo. Ya que, de otro modo, se desactiva inmediatamente la animación
            Invoke("CheckGroundedAfterJump", 0.1f);
        }

        if (attacking)
        {
            if (isOnRange(enemy) && enemy.activeSelf)
            {
                moving = false;
                objectMoving = null;
                naeveAnimator.SetTrigger("attack");
                attacking = false;
                Debug.Log("No me puedes hacer daño");
            }
        }
        else
        {
            // Si el enemigo está lo suficientemente cerca, el enemigo está activo, el personaje y el enemigo están en la misma capa y el personaje no es invisible, se activa la muerte
            if (isOnRange(enemy) && enemy.activeSelf && enemy.layer == player.layer && !isInvisible)
            {
                NaeveDeath();
            }
        }
    }

    public void ActivateParaguas()
    {
        paraguasActivated = true;
    }

    public void DeactivateParaguas()
    {
        paraguasActivated = false;
    }

    // Función para checkear si Naeve ha tocado ya el suelo y podemos quitar la animación de saltar
    private void CheckGroundedAfterJump()
    {
        if (IsGrounded())
        {
            naeveAnimator.SetBool("isJump", false);
            isJumping = false;
        }
    }

    // Cogemos los prompts de cada GPT y los activamos en SendAndHandleReply
    private async void GenerateErrorGPT()
    {
        string errorPrompt = promptManager.getErrorPrompt();
        errorControllerGPT.SetPrompt(errorPrompt);
        await SendAndHandleReplyError("");
    }

    private async Task GeneratePortonGPT()
    {
        string portonMsg = promptManager.getGatePrompt();
        portonControllerGPT.SetPrompt(portonMsg);
        Debug.Log("Prompt seteado, continuamos: " + portonMsg);
        await SendAndHandleReplyNPC(portonControllerGPT, "");
    }

    private async Task GenerateEstatuaGPT()
    {
        string estatuaMsg = promptManager.getStatuePrompt();
        estatuaControllerGPT.SetPrompt(estatuaMsg);
        Debug.Log("Prompt seteado, continuamos: " + estatuaMsg);
        await SendAndHandleReplyNPC(estatuaControllerGPT, "");
    }

    private async Task GenerateQuimeraGPT()
    {
        string quimeraMsg = promptManager.getQuimeraPrompt();
        quimeraControllerGPT.SetPrompt(quimeraMsg);
        Debug.Log("Prompt seteado, continuamos: " + quimeraMsg);
        await SendAndHandleReplyNPC(quimeraControllerGPT, "");
    }

    private async Task GenerateLoboGPT()
    {
        string loboMsg = promptManager.getLoboPrompt();
        loboControllerGPT.SetPrompt(loboMsg);
        Debug.Log("Prompt seteado, continuamos: " + loboMsg);
        await SendAndHandleReplyNPC(loboControllerGPT, "");
    }

    private async Task GenerateGuardaGPT()
    {
        string guardaMsg = promptManager.getGuardaPrompt();
        guardaControllerGPT.SetPrompt(guardaMsg);
        Debug.Log("Prompt seteado, continuamos: " + guardaMsg);
        await SendAndHandleReplyNPC(guardaControllerGPT, "");
    }

    private async void InitializePorton()
    {
        if (portonControllerGPT != null)
        {
            //Debug.Log("Todo encontrado, continuamos");

            await GeneratePortonGPT();
        }
    }

    private async void InitializeEstatua()
    {
        if (estatuaControllerGPT != null)
        {
            await GenerateEstatuaGPT();
        }
    }

    private async void InitializeQuimera()
    {
        if (quimeraControllerGPT != null)
        {
            await GenerateQuimeraGPT();
        }
    }

    private async void InitializeLobo()
    {
        if (loboControllerGPT != null)
        {
            await GenerateLoboGPT();
        }
    }

    private async void InitializeGuarda()
    {
        if (guardaControllerGPT != null)
        {
            await GenerateGuardaGPT();
        }
    }


    // Construye el prompt cuando aparece el hombre del sombrero
    private async void buildEnemyPrompt()
    {
        string msg = promptManager.getEnemyPrompt();

        if (inventory.Count == 0)
        {
            msg += "Inventario vacío.";
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
        string sendMsg = "El jugador ha hecho click en " + hit.collider.gameObject.name + ", en la posición: " + hit.point;
        Debug.Log(hit.point);
        await SendAndHandleReply(sendMsg);
    }

    //Creamos un mensaje cualquiera pasado por parámetro
    private async void buildOtherMsg(string msg)
    {
        await SendAndHandleReply(msg);
    }

    // Función para mover a Naeve a un objetivo determinado
    private void MoveTowardsTarget(GameObject obj)
    {
        float step = speed * Time.deltaTime;
        obj.transform.position = Vector2.MoveTowards(obj.transform.position, target, step);

        // Si Naeve estaba escondida, deja de estarlo cambiando la capa.
        if (hidden)
        {
            // Volvemos a cambiar las layers a Default
            logicController.ChangeLayer(player.transform, 9, "Naeve");
            hidden = false;
        }

        if ((Vector2)objectMoving.transform.position == target)
        {
            moving = false;
            objectMoving = null;
            // Si el objeto en movimiento es el jugador, quitamos la animación del movimiento.
            if (obj == player)
            {
                naeveAnimator.SetBool("isRun", false);
            }
            // Si estábamos escondiéndonos, volvemos a poner la velociad normal y cambiamos la capa.
            if (hidde == true)
            {
                // Cambiamos la capa de Naeve para esconderla tras los objetos
                logicController.ChangeLayer(player.transform, 8, "hidden");
                SetSpeed(10f);
                // Naeve ya está escondida
                hidde = false;
                hidden = true;
            }
        }
    }

    private void TeleportTowardsTarget(GameObject obj)
    {
        obj.transform.position = target;
    }
    // Vuelve invisible al objeto paulatinamente
    //private void InvisibleTarget(GameObject obj)
    //{
    //    // Obtenemos el renderer del objeto
    //    Renderer objectRenderer = obj.GetComponent<Renderer>();
    //    // Lerp (interpolación lineal) entre la transparencia actual y 0 (totalmente transparente)
    //    float currentTransparency = objectRenderer.material.color.a;
    //    float newTransparency = Mathf.Lerp(currentTransparency, 0.02f, fadeSpeed * Time.deltaTime);
    //    // Actualiza el color del material con la nueva transparencia
    //    Color newColor = objectRenderer.material.color;
    //    newColor.a = newTransparency;
    //    objectRenderer.material.color = newColor;

    //    if (newTransparency <= invisibleValue)
    //    {
    //        invisible = false;
    //        objectInvisible = null;
    //    }
    //}

    private void InvisibleTarget(GameObject obj)
    {
        // Busca el objeto hijo "Skeletal"
        UnityEngine.Transform skeletal = obj.transform.Find("Skeletal");
        if (skeletal == null)
        {
            Debug.LogError("No se encontró el objeto hijo 'Skeletal'");
            return;
        }

        // Itera sobre todos los hijos de "Skeletal"
        foreach (UnityEngine.Transform child in skeletal)
        {
            // Obtiene el renderer del hijo actual
            Renderer childRenderer = child.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                // Lerp (interpolación lineal) entre la transparencia actual y 0 (totalmente transparente)
                float currentTransparency = childRenderer.material.color.a;
                float newTransparency = Mathf.Lerp(currentTransparency, 0.02f, fadeSpeed * Time.deltaTime);

                // Actualiza el color del material con la nueva transparencia
                Color newColor = childRenderer.material.color;
                newColor.a = newTransparency;
                childRenderer.material.color = newColor;

                if (newTransparency <= invisibleValue)
                {
                    invisible = false;
                    objectInvisible = null;
                    isInvisible = true;
                }
            }
        }
    }

    private void Jump(Rigidbody2D rb)
    {
        if (IsGrounded() & !naeveAnimator.GetBool("isJump")) // Asegura que el jugador esté en el suelo antes de saltar y que no está la animación ya activada
        {
            isJumping = true;
            naeveAnimator.SetBool("isJump", true);
            rb.velocity = new Vector2(rb.velocity.x, 0f); // Reinicia la velocidad en el eje Y
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private bool IsGrounded()
    {
        // Implementa la lógica para verificar si el jugador está en el suelo
        float raycastDistance = 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.down, raycastDistance, groundLayerMask);

        return hit.collider != null;
    }

    private async Task SendAndHandleReply(string msgSend)
    {
        reply = await naeveControllerGPT.SendReply(msgSend);
        //Debug.Log("get answer 1: " + reply);
        while (string.IsNullOrEmpty(reply)) // Esperar hasta que la respuesta no esté vacía
        {
            await Task.Delay(10);
        }

        // Comprobamos que el formato sea correcto
        //await SendAndHandleReplyError(reply);
        //GetErrorReply();

        naeveText = reply;

        string actionReply = await actionControllerGPT.SendReply(msgSend + "\n" + reply);
        Debug.Log("ACCION: " + actionReply);

        // Parseamos el mensaje y obtenemos la acción
        GetAction(actionReply);

        // Eliminamos contenido innecesario del mensaje de la narración de Naeve.
        FormatNaeveText();

        naeveControllerGPT.AppendMessage(naeveText);
        actionControllerGPT.AppendMessage(actionText);

        // Llama a traduce para traducir el mensaje a una acción.
        InterpretString(actionText);
        naeveControllerGPT.UpdateGPT();
        // Resetea reply para futuras solicitudes
        reply = "";
    }

    private async Task SendAndHandleReplyError(string msg)
    {
        errorReply = await errorControllerGPT.SendReply(msg);
        while (string.IsNullOrEmpty(errorReply)) // Esperar hasta que la respuesta no esté vacía
        {
            await Task.Delay(10);
        }
        // Guardamos la posible corrección por si es necesario enviarla
        correction = errorReply;
        errorControllerGPT.AppendMessage(errorReply);
        errorControllerGPT.UpdateGPT();
        errorReply = "";
    }

    private async Task SendAndHandleReplyNPC(GPTController chatController, string msg)
    {
        chatReply = await chatController.SendReply(msg);
        while (string.IsNullOrEmpty(chatReply)) // Esperar hasta que la respuesta no esté vacía
        {
            await Task.Delay(10);
        }
        Debug.Log(chatController.tag + ": " + chatReply);
        chatController.AppendMessage(chatReply);
        chatController.UpdateGPT();
        // Si el NPC es el portón buscamos la cadena "[Abierta]"
        // Si es el primer mensaje no busco las key por si al GPT se le va la olla y decide contestar con el comando para explicarse. (Ha pasado muchas veces)
        if (!chatController.GetFirstMsg())
        {
            if (chatController == portonControllerGPT)
            {
                GetPortonKey(chatReply);
            }
            else if (chatController == estatuaControllerGPT)
            {
                GetEstatuaKey(chatReply);
            }
            else if (chatController == quimeraControllerGPT)
            {
                GetQuimeraKey(chatReply);
            }
            else if (chatController == loboControllerGPT)
            {
                GetLoboKey(chatReply);
            }
            else if (chatController == guardaControllerGPT)
            {
                GetGuardaKey(chatReply);
            }
        }
        
        chatReply = "";
    }

    private void InitializeGameObjectDictionary()
    {
        objectDictionary.Add("paraguas", paraguas);
        objectDictionary.Add("cómic", comic);
        objectDictionary.Add("tronco", tronco);
        objectDictionary.Add("sofá", sofa);
        objectDictionary.Add("mesa", mesa);
        objectDictionary.Add("vela", vela);
        objectDictionary.Add("silla", silla);
        objectDictionary.Add("naeve", player);
        objectDictionary.Add("enemigo", enemy);
        objectDictionary.Add("portón", porton);
        objectDictionary.Add("cofre", cofre);
        objectDictionary.Add("Naeve", player);
        objectDictionary.Add("estatua", estatua);
        objectDictionary.Add("árbol", arbol);
        objectDictionary.Add("quimera", quimera);
        objectDictionary.Add("lobo", lobo);
        objectDictionary.Add("guarda", guarda);
    }

    private void InitializeActionObjectLogic()
    {
        // PENSAR SOBRE SI PEDIR ALGÚN PARÁMETRO MÁS A LA HORA DE MENGUAR, CRECER, TRANSFORMAR, LEVITAR...
        // Asignación de lógica a combinaciones específicas con métodos que aceptan GameObject como argumento
        actionObjectLogic.Add("mover", Mover); // HECHO
        actionObjectLogic.Add("coger", Coger); // HECHO
        actionObjectLogic.Add("transformar", Transformar);
        actionObjectLogic.Add("vibrar", Vibrar);
        actionObjectLogic.Add("desaparecer", Desaparecer); // HECHO
        actionObjectLogic.Add("menguar", Menguar); // HECHO
        actionObjectLogic.Add("crecer", Crecer); // HECHO
        actionObjectLogic.Add("explotar", Explotar); // HECHO
        actionObjectLogic.Add("atacar", Atacar); // HECHO
        actionObjectLogic.Add("esconderse", Esconderse); // HECHO
        actionObjectLogic.Add("atraer", Atraer); // HECHO
        actionObjectLogic.Add("teletransportar", Teletransportar); // HECHO
        actionObjectLogic.Add("soltar", Soltar); // HECHO
        actionObjectLogic.Add("levitar", Levitar); // HECHO
        actionObjectLogic.Add("materializar", Materializar); // HECHO
        actionObjectLogic.Add("utilizar", Utilizar);
        actionObjectLogic.Add("saltar", Saltar); // HECHO
        actionObjectLogic.Add("hablar", Hablar);
        actionObjectLogic.Add("esperar", Esperar); // HECHO
        actionObjectLogic.Add("caer", Caer); // HECHO
        actionObjectLogic.Add("invisibilizar", Invisibilizar); // HECHO
        actionObjectLogic.Add("controlar", Controlar); // HECHO
    }

    private void ExecuteActionObjectLogic(string action, GameObject obj)
    {
        string key = action;
        if (actionObjectLogic.ContainsKey(key))
        {
            actionObjectLogic[key].Invoke(obj);
        }
        else
        {
            Debug.Log("No se encontró lógica para la acción: " + key);
        }
    }

    private void Invisibilizar(GameObject obj)
    {
        if (obj != null)
        {
            if (!invisible)
            {
                invisible = true;
                objectInvisible = obj;
                InvisibleTarget(obj);
            }
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }
    }

    private void Controlar(GameObject obj)
    {
        if (obj != null)
        {
            if (!PlayerMovement.playerControl)
            {
                PlayerMovement.playerControl = true;
            }
            else
            {
                PlayerMovement.playerControl = false;
            }

        }
    }

    // Si el objeto tiene rigidbody que está desactivado, lo activa. De esta forma el objeto caerá
    private void Caer(GameObject obj)
    {
        if (obj != null)
        {
            Rigidbody2D body = obj.GetComponent<Rigidbody2D>();
            if (body != null)
            {
                if (obj.GetComponent<Rigidbody2D>().IsSleeping())
                {
                    obj.GetComponent<Rigidbody2D>().WakeUp();
                }
            }
            else
            {
                Debug.Log("Lógica para caer el objeto: " + obj.name);
                // Hacemos que el objeto dado se mueva a la posición posx, posy, dada por chatGPT y guardada en la función anterior
                target = new Vector2(obj.transform.position.x, -0.9f); // El target en este caso sería el suelo
                moving = true;
                objectMoving = obj;
                MoveTowardsTarget(obj);
            }
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }

    }

    private void Esperar(GameObject obj)
    {
        Debug.Log("Naeve está esperando");
    }

    private void Hablar(GameObject obj)
    {
        if (obj != null)
        {
            Debug.Log("Lógica para hablar con el ente: " + obj.tag);
            //if (isOnRange(obj))
            //{
            switch (obj.tag)
            {
                case "Naeve":
                    Debug.Log("Estás hablando contigo misma");
                    break;
                case "enemigo":
                    Debug.Log("Estás hablando con el hombre del sombrero");
                    break;
                case "portón":
                    Debug.Log("Estás hablando con el portón");
                    TalkManager.Instance.WakeUpPortonMenu();
                    //InitializePorton();
                    break;
                case "estatua":
                    Debug.Log("Estás hablando con la estatua");
                    TalkManager.Instance.WakeUpEstatuaMenu();
                    break;
                case "quimera":
                    Debug.Log("Estás hablando con la quimera");
                    TalkManager.Instance.WakeUpQuimeraMenu();
                    break;
                case "lobo":
                    Debug.Log("Estás hablando con el lobo");
                    TalkManager.Instance.WakeUpLoboMenu();
                    break;
                case "guarda":
                    Debug.Log("Estás hablando con el guarda");
                    TalkManager.Instance.WakeUpGuardaMenu();
                    break;
                default:
                    Debug.Log("La entidad no existe");
                    break;
            }

            //}
            //else
            //{
            //    string sendMsg = "Estás muy lejos del objeto. Muévete cerca del objeto antes";
            //    buildOtherMsg(sendMsg);
            //    Debug.Log("Acercate al objeto antes");
            //}
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }

    }

    private void Saltar(GameObject obj)
    {
        if (obj != null)
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            Jump(rb);
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }

    }

    // Métodos con lógica para acciones específicas que aceptan un GameObject como argumento
    private void Utilizar(GameObject obj)
    {
        if (obj != null)
        {
            utilizarController.Utilizar(obj);
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }
    }

    private void Materializar(GameObject obj)
    {
        if (obj != null)
        {
            Vector3 spawnPos;
            if (cofreActivado)
            {
                spawnPos = cofre.transform.position + 4 * (Vector3.up);
                cofreActivado = false;
            }
            else
            {
                // Instancio el prefab al lado de Naeve
                spawnPos = player.transform.position + 5 * (Vector3.right + Vector3.up);
            }
            Instantiate(obj, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }

    }

    private void Levitar(GameObject obj)
    {
        if (obj != null)
        {
            Debug.Log("Lógica para hacer levitar el objeto: " + obj.name);
            // Hacemos que el objeto dado se mueva a la posición posx, posy, dada por chatGPT y guardada en la función anterior
            Rigidbody2D body = obj.GetComponent<Rigidbody2D>();
            if (body != null)
            {
                body.bodyType = RigidbodyType2D.Static;
            }
            target = obj.transform.position; // El target pasa a ser un punto por encima del objeto
            target.y = obj.transform.position.y + 10f;
            moving = true;
            objectMoving = obj;
            MoveTowardsTarget(obj);
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }

    }


    // Suelta el objeto en el mismo lugar en el que se cogió, ya que sólo lo desactivamos y lo activamos.
    private void Soltar(GameObject obj)
    {
        if (obj != null)
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
                Debug.Log("El objeto que quieres soltar no está en el inventario");
            }
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }


    }

    // Teletransporta a Naeve al objeto obj
    private void Teletransportar(GameObject obj)
    {
        if (obj != null)
        {
            Debug.Log("Lógica para teletransportar el objeto: " + obj.name);
            // Hacemos que el objeto dado se mueva a la posición posx, posy, dada por chatGPT y guardada en la función anterior
            // Si la altura es menor que el suelo, teletransportamos al suelo.
            //posY = (posY < 2.2525f) ? 2.2525f : posY;
            if (posY < 2.2525f) posY = 2.2525f;
            target = new Vector2(posX, posY); // Creamos el vector 2D con las posiciones del objeto
                                              // Añado Vextor3.up * 5.0f para que el efecto sea en el cuerpo de Naeve y no en los pies
            Instantiate(teleportVFX, obj.transform.position + Vector3.up * 4.0f, Quaternion.identity);
            TeleportTowardsTarget(obj);
            Instantiate(teleportVFX, target, Quaternion.identity);
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }

    }

    private void Atraer(GameObject obj)
    {
        if (obj != null)
        {
            Debug.Log("Lógica para atraer el objeto: " + obj.name);
            // Hacemos que el objeto dado se mueva a la posición posx, posy, dada por chatGPT y guardada en la función anterior
            target = (Vector2)player.transform.position; // El target en este caso sería la propia Naeve
            moving = true;
            objectMoving = obj;
            MoveTowardsTarget(obj);
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }

    }

    private void Esconderse(GameObject obj)
    {
        if (obj != null)
        {
            if (obj == enemy) obj = sofa; // Una pequeña trampita por si decide responder que se quiere esconder del enemigo con /esconder/enemigo queriendo esconderse de este no en este. Pongo el sofá como escondite "default"
            // Movemos a Naeve a ese objeto. Subo la velocidad para que vaya corriendo, ya que se trata de una urgencia.
            target = (Vector2)obj.transform.position;
            moving = true;
            objectMoving = player;
            SetSpeed(15f);
            hidde = true;

            if (IsGrounded())
            {
                MoveNaeve();
                MoveTowardsTarget(player);
            }
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }

    }

    private void Atacar(GameObject obj)
    {
        if (obj != null)
        {
            if (inventory.Contains("cuchillo"))
            {
                GameObject naeve = GameObject.Find("Naeve");

                if (naeve != null)
                {
                    // Si se encuentra "Naeve", buscar dentro de él el objeto "Skeletal"
                    UnityEngine.Transform skeletalTransform = naeve.transform.Find("Skeletal");

                    if (skeletalTransform != null)
                    {
                        // Si se encuentra "Skeletal", buscar dentro de él el objeto "Cuchillo"
                        GameObject cuchillo = skeletalTransform.Find("Cuchillo").gameObject;

                        if (cuchillo != null)
                        {
                            // Si se encuentra "Cuchillo", activarlo
                            cuchillo.SetActive(true);
                        }
                        else
                        {
                            Debug.LogError("El objeto 'Cuchillo' no se ha encontrado dentro de 'Skeletal'.");
                        }
                    }
                    else
                    {
                        Debug.LogError("El objeto 'Skeletal' no se ha encontrado dentro de 'Naeve'.");
                    }
                }
                else
                {
                    Debug.LogError("El objeto 'Naeve' no se ha encontrado en la escena.");
                }

                target = (Vector2)obj.transform.position;
                moving = true;
                objectMoving = player;

                if (IsGrounded())
                {
                    attacking = true;
                    objAttacked = obj;
                    MoveNaeve();
                    MoveTowardsTarget(player);
                }
            }
            else
            {
                Debug.Log("No tienes ningún objeto para atacar");
            }
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }

    }

    private void Explotar(GameObject obj)
    {
        if (obj != null)
        {
            Debug.Log("Explotando...");
            // Instanciamos el efecto de la explosión y aumentamos su tamaño
            GameObject explosionInstance = Instantiate(explosionVFX, obj.transform.position, Quaternion.identity);
            Vector3 nuevaEscala = new Vector3(10f, 10f, 10f);
            explosionInstance.transform.localScale = nuevaEscala;
            Desaparecer(obj);
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }

    }

    // Quizá en la función crecer, pedir a chat gpt cuanto tiene que crecer.
    private void Crecer(GameObject obj)
    {
        if (obj != null)
        {
            float newX = obj.transform.localScale.x * 2;
            float newY = obj.transform.localScale.y * 2;
            StartCoroutine(CambiarTamañoConTransicion(obj, newX, newY));
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }

    }

    private void Menguar(GameObject obj)
    {
        if (obj != null)
        {
            float newX = obj.transform.localScale.x / 2;
            float newY = obj.transform.localScale.y / 2;
            StartCoroutine(CambiarTamañoConTransicion(obj, newX, newY));
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }

    }

    private void Desaparecer(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(false);
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }

    }

    private void Vibrar(GameObject obj)
    {
        throw new NotImplementedException();
    }

    private void Transformar(GameObject obj)
    {
        throw new NotImplementedException();
    }

    private void MoveNaeve()
    {
        target.y = player.transform.position.y;
        // Si el target está a la izquierda, Naeve correría hacia la izquierda. Si no, hacia la derecha. Utilizo la posición x del vector en negativo o positivo para hacer esto
        // Además, como la escala de Naeve es de 2.2, además del - o + en la dirección, mantenemos la escala de 2.2
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

    void Mover(GameObject obj)
    {
        if (obj != null)
        {
            Debug.Log("Lógica para mover el objeto: " + obj.name);
            // Hacemos que el objeto dado se mueva a la posición posx, posy, dada por chatGPT y guardada en la función anterior
            target = new Vector2(posX, posY); // Creamos el vector 2D con las posiciones del objeto
            posX = 0; posY = 0; // * Reseteo posiciones para depuración
            moving = true;
            objectMoving = obj;
            if (obj == player && IsGrounded()) // Si el objeto dado es el jugador sólo se puede mover en el eje x, además activamos la animación de correr
            {
                MoveNaeve();
            }
            if (IsGrounded()) MoveTowardsTarget(obj);
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }

    }

    private void Coger(GameObject obj)
    {
        if (obj != null)
        {
            Debug.Log("Lógica para coger el objeto: " + obj.name);
            if (isOnRange(obj))
            {
                inventory.Add(obj.tag); // Añadimos el objeto al inventario

                // Obtenemos el script para añadir el objeto al inventario y llamamos a la función que lo hace
                ItemPickUp pickUp = obj.GetComponent<ItemPickUp>();
                if (pickUp != null)
                {
                    pickUp.Pickup();
                }
                //Destroy(obj);
                Debug.Log("Añadido: " + inventory[inventory.Count - 1] + " al inventario.");

                // Enviamos a GPT la información con el inventario actualizado.
                string msg = obj.tag + " añadido al inventario. Objetos en el inventario: ";
                foreach (string item in inventory)
                {
                    msg += item;
                }
                //buildOtherMsg(msg);
            }
            else
            {
                string sendMsg = "Estás muy lejos del objeto. Muévete cerca del objeto antes";
                //buildOtherMsg(sendMsg);
                Debug.Log("Acercate al objeto antes");
            }
        }
        else
        {
            Debug.Log("El objeto ha sido destruido");
        }

    }

    // Utilizo una corutina para ir menguando y creciendo poco a poco el objeto
    IEnumerator CambiarTamañoConTransicion(GameObject obj, float newX, float newY)
    {
        float timeSpent = 0f;
        float transitionTime = 2f;

        Vector3 iniSize = obj.transform.localScale;
        Vector3 newSize = new Vector3(newX, newY, obj.transform.localScale.z);

        while (timeSpent < transitionTime)
        {
            timeSpent += Time.deltaTime;

            // Interpola suavemente entre el tamaño inicial y el nuevo tamaño
            obj.transform.localScale = Vector3.Lerp(iniSize, newSize, timeSpent / transitionTime);

            yield return null;
        }

        // Asegura que el tamaño final sea el que queremos
        obj.transform.localScale = newSize;
    }

    private bool isOnRange(GameObject obj)
    {
        float distance = Vector2.Distance(player.transform.position, obj.transform.position);
        if (distance <= interactionRange) return true;
        return false;
    }

    public void GetPortonKey(string message)
    {
        string pattern = @"\[Abierta\]";
        // Busco la primera coincidencia de la cadena "[Abierta]" en el mensaje del portón 
        Match match = Regex.Match(message, pattern);

        if (match.Success)
        {
            portalVFX.SetActive(true);
            Debug.Log("Abierta!");
            // Funcionalidad de abrir la puerta y acabar el juego. Final 1.
        }
    }

    public void GetEstatuaKey(string message)
    {
        // Patrón para encontrar una cadena entre corchetes que representaría al objeto a crear
        string pattern = @"\[[^\]]+\]";
        Match match = Regex.Match(message, pattern);

        if (match.Success)
        {
            Debug.Log("Objeto encontrado, comprobamos si existe");
            string keyObject = match.Value.Trim('[', ']').ToLower(); // Obtengo la clave sin los corchetes
            GameObject objectParsed = GetPrefabToSpawn(keyObject);
            if (objectParsed != null)
            {
                cofreActivado = true;
                ChestAnimator chestAnimator = cofre.GetComponent<ChestAnimator>();
                if (chestAnimator != null) chestAnimator.OpenChest();
                Materializar(objectParsed);
            }
            else
            {
                Debug.Log("Objeto no existente");
            }
            // Funcionalidad de materializar el objeto
        }
    }

    public void GetQuimeraKey(string message)
    {
        string pattern = @"\[Adelante\]";
        // Busco la primera coincidencia de la cadena "[Abierta]" en el mensaje del portón 
        Match match = Regex.Match(message, pattern);
        if (match.Success)
        {
            Debug.Log("Puedes continuar tu camino!");
            Invoke("DesactivarQuimera", 5f); // Llama al método DesactivarQuimera después de 5 segundos
        }
    }

    void DesactivarQuimera()
    {      
        TalkButtonController buttonQuimera = quimera.GetComponent<TalkButtonController>();
        if (buttonQuimera != null)
        {
            Instantiate(teleportVFX, quimera.transform.position + Vector3.up * 4.0f, Quaternion.identity);
            buttonQuimera.Deactive();
            quimeraControllerGPT = null;
            quimera.SetActive(false); // Desactiva el GameObject quimera
        }
        
    }

    public void GetLoboKey(string message)
    {
        string pattern = @"\[Seguir\]";
        // Busco la primera coincidencia de la cadena "[Abierta]" en el mensaje del portón 
        Match match = Regex.Match(message, pattern);
        if (match.Success)
        {
            Debug.Log("El lobito te está siguiendo!");
            // Funcionalidad para que el lobo siga al jugador
            WolfController wolfController = lobo.GetComponent<WolfController>();
            if (wolfController != null)
            {
                wolfController.SetFollow(true);
            }
        }
    }

    public void GetGuardaKey(string message)
    {
        string pattern1 = @"\[Alarma\]";
        string pattern2 = @"\[Irse\]";

        // Busco la primera coincidencia de la cadena "[Abierta]" en el mensaje del portón 
        Match match1 = Regex.Match(message, pattern1);
        Match match2 = Regex.Match(message, pattern2);

        if (match1.Success)
        {
            Debug.Log("El guarda ha activado la alarma!");
            // Funcionalidad para que el lobo siga al jugador
            GuardaController guardaController = guarda.GetComponent<GuardaController>();
            if (guardaController != null)
            {
                guardaController.Alarma();
            }
        }
        else if (match2.Success)
        {
            Debug.Log("El guarda ha activado te ha dejado en paz!");
            // Funcionalidad para que el lobo siga al jugador
            GuardaController guardaController = guarda.GetComponent<GuardaController>();
            if (guardaController != null)
            {
                guardaController.Irse();
            }
        }
    }


    public void GetAction(string message)
    {
        Debug.Log(message);
        // Inicialización de las variables de salida
        actionText = string.Empty;

        // Elimina la cadena "/accion" del mensaje ante la cantidad de errores que cometía de esta índole
        List<string> palabrasAEliminar = new List<string> { "/accion", "/acción", "/Acción", "/Accion" };

        // Itera sobre cada palabra en la lista y sustitúyela por una cadena vacía en el mensaje
        foreach (string palabra in palabrasAEliminar)
        {
            message = message.Replace(palabra, "");
        }

        // Ajuste en la expresión regular para capturar adecuadamente el patrón deseado.
        // Esta expresión captura '/Comando/parametros' seguido opcionalmente por otros caracteres,
        // pero solo nos interesan los grupos capturados antes del espacio o fin de la cadena.
        var match = Regex.Match(message, @"(\/\w+\/[^ \r\n]*)");

        // Ahora 'actionText' contendrá solo el texto que coincide con el patrón deseado.
        actionText = match.Success ? match.Groups[1].Value : "";

        // Limpieza final para eliminar caracteres no deseados, si es necesario.
        // Esta línea puede ser opcional dependiendo de si tu expresión regular ya asegura el formato deseado.
        actionText = Regex.Replace(actionText, @"[^\w\/,.]", "");

        actionText = actionText.TrimEnd(',', ' ', '.', '"', '-', '_', '*', '/');


        // Usar expresiones regulares para encontrar la acción y el parámetro si existe
        //var match = Regex.Match(message, @"\/(\w+)(\/[^ ]*)?");

        //var match = Regex.Match(message, @"\/(\w+)(\/[^ ]*)?");

        //var pattern = @"\/\w+(\/\w+)?(\/\d+,\d+)?";
        //var matches = Regex.Matches(message, pattern);

        //foreach (Match match in matches)
        //{
        //    Debug.Log("Comando encontrado: " + match.Value);
        //}

        // Extraer el comando completo
        //actionText = match.Value;

        //int spaceIndex = actionText.IndexOf(' ');
        //if (spaceIndex != -1)
        //{
        //    // Si hay un espacio, cortar la cadena hasta ese punto
        //    actionText = actionText.Substring(0, spaceIndex);
        //}
        //// Eliminamos cualquier símbolo residual producto de la alucinación
        //actionText = Regex.Replace(actionText, @"[^\w\d/,.]", "");

        //actionText = actionText.TrimEnd(',', ' ', '.', '"', '-', '_', '*');

        Debug.Log("Acciones: " + actionText);
    }

    //public void GetAction(string message, out string actionText, out string naiveText)
    //{
    //    Debug.Log(message);
    //    // Inicialización de las variables de salida
    //    actionText = string.Empty;
    //    naiveText = string.Empty;

    //    // Usar expresiones regulares para encontrar la acción y el parámetro si existe
    //    var match = Regex.Match(message, @"\/(\w+)(\/[^ ]*)?");

    //    if (!match.Success)
    //    {
    //        // Si no hay acción, todo el mensaje se considera texto en lenguaje natural
    //        naiveText = message;
    //        return;
    //    }

    //    // Extraer el comando completo
    //    actionText = match.Value;

    //    // Remover el comando del mensaje original para obtener el texto en lenguaje natural
    //    naiveText = Regex.Replace(message, Regex.Escape(match.Value), "").Trim();

    //    // Asegurar que el texto en lenguaje natural no comience ni termine con espacios innecesarios
    //    naiveText = naiveText.TrimEnd(',', ' ', '.');
    //    actionText = actionText.TrimEnd(',', ' ', '.');

    //    Debug.Log("Texto restante final: " + naeveText);
    //    Debug.Log("Acciones: " + actionText);
    //}

    // Separamos el texto que es de acciones de Naeve y el hablado. Utilizo parámetros de salida para devolver ambos textos.
    //public void getAction(string message, out string actionText, out string naeveText)
    //{
    //    actionText = "";
    //    naeveText = message;

    //    int indiceCorcheteApertura = -1;
    //    int indiceCorcheteCierre = -1;

    //    while (true)
    //    {
    //        indiceCorcheteApertura = naeveText.IndexOf('[', indiceCorcheteApertura + 1);
    //        indiceCorcheteCierre = naeveText.IndexOf(']', indiceCorcheteCierre + 1);

    //        if (indiceCorcheteApertura != -1 && indiceCorcheteCierre != -1 && indiceCorcheteCierre > indiceCorcheteApertura)
    //        {
    //            string textoEntreCorchetes = naeveText.Substring(indiceCorcheteApertura + 1, indiceCorcheteCierre - indiceCorcheteApertura - 1);
    //            string textoAntesDeCorchetes = naeveText.Substring(0, indiceCorcheteApertura);
    //            string textoDespuesDeCorchetes = naeveText.Substring(indiceCorcheteCierre + 1).Trim();

    //            //Debug.Log("Texto entre corchetes: " + textoEntreCorchetes);
    //            //Debug.Log("Texto antes de corchetes: " + textoAntesDeCorchetes);
    //            //Debug.Log("Texto después de corchetes: " + textoDespuesDeCorchetes);

    //            // CUIDADO POR SI ELIMINO ALGO QUE NO TOCA
    //            naeveText = textoAntesDeCorchetes.TrimEnd(',', ' ') + textoDespuesDeCorchetes.TrimEnd(',', ' ');
    //            actionText = actionText + "/" + textoEntreCorchetes;

    //            indiceCorcheteApertura = -1;
    //            indiceCorcheteCierre = -1;
    //        }
    //        else
    //        {
    //            break;
    //        }
    //    }

    //    Debug.Log("Texto restante final: " + naeveText);
    //    Debug.Log("Acciones: " + actionText);
    //}

    public void InterpretString(string inputString)
    {
        Debug.Log("Has entrado en el interprete");
        string[] parts = inputString.Split('/');

        if (parts.Length >= 3)
        {
            string action = parts[1].ToLower();
            string[] subParts;

            // Si el formato es /accion/objeto, si hay un comando extra para marcar la posición /posx,posy, estará en la posición 3, si el formato es /accion, estará en la 2
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
                string objeto = parts[2].ToLower();
                Debug.Log("Acción: " + action);
                Debug.Log("Objeto: " + objeto);

                GameObject objectParsed;

                if (action == actions[14]) // Si la acción es materializar, la función es un poco diferente, por lo que lo trato como un caso aparte
                {
                    Debug.Log("Estamos dentro de la acción");
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
                    // Si no es uno de los objetos predeterminados de la escena, puede haber sido creado por materializa, por lo que tendría que probar y ejecutarlo en ese caso
                    Debug.Log("Error: No se ha encontrado el objeto");
                }
            }
            else if (parts.Length == 3 && subParts.Length == 2) // Formato: /accion/posicionx,posiciony
            {
                // Parseamos las coordenadas si es posible, y las guardamos en los atributos de posicion
                // Dado que el formato que le mando a hactgpt es 8.9 en vez de 8,9, tengo que añadir NumberStyles.Float, CultureInfo.InvariantCulture
                // Puedo cambiar el formato pedido y ahorrarme esto
                if (float.TryParse(subParts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out posX) && float.TryParse(subParts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out posY))
                {
                    Debug.Log("Acción: " + action);
                    Debug.Log("Posición X: " + posX);
                    Debug.Log("Posición Y: " + posY);
                    if (action == "avanzar") action = "mover";
                    // Confirmamos que la accion sea mover
                    if (action == actions[1])
                    {
                        // En este caso, tenemos sólo una orden de mover y la posición, por lo que hay que mover a Naeve(player)
                        ExecuteActionObjectLogic(action, player);
                    }
                }
                else
                {
                    //errorTrigger = true;
                    Debug.LogWarning("Formato de posición no válido.");
                }
            }
            else if (parts.Length == 4 && subParts.Length == 1) // Formato: /accion/objeto/objeto
            {
                string objeto1 = parts[2].ToLower();
                string objeto2 = parts[3].ToLower();
                Debug.Log("Acción: " + action);
                Debug.Log("Objeto 1: " + objeto1);
                Debug.Log("Objeto 2: " + objeto2);
                // Por ahora no he conseguido implementar esto por tener dos argumentos la función (objeto1, objeto2)
            }
            else if (parts.Length == 4 && subParts.Length == 2) // Formato: /accion/objeto/posicionx,posiciony
            {
                string objeto = parts[2].ToLower();
                if (float.TryParse(subParts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out posX) && float.TryParse(subParts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out posY))
                {
                    Debug.Log("Acción: " + action);
                    Debug.Log("Objeto: " + objeto);
                    Debug.Log("Posición X: " + posX);
                    Debug.Log("Posición Y: " + posY);

                    GameObject objectParsed;
                    // Si la acción es teletransportar y el objeto está en el diccionario de objetos y entes
                    if (/*action == actions[11] && */objectDictionary.TryGetValue(objeto, out objectParsed))
                    {
                        // En este caso, tenemos sólo una orden de teletransportar, el objeto o ente y la posición.
                        ExecuteActionObjectLogic(action, objectParsed);
                    }
                }
                else
                {
                    //errorTrigger = true;
                    Debug.LogWarning("Formato de posición no válido.");
                }
            }
            else
            {
                //errorTrigger = true;
                Debug.LogWarning("Formato no reconocido.");
            }
        }
        else if (parts.Length == 2)
        {
            string action = parts[1].ToLower();
            // Nos aseguramos de que la acción sea \esperar o \saltar
            ExecuteActionObjectLogic(action, player);
            //if (action == actions[18] || action == actions[16])
            //{
            //    ExecuteActionObjectLogic(action, player);
            //}
        }
        else if (string.IsNullOrEmpty(inputString))
        {
            Debug.Log("No se ha realizado ninguna acción");
        }
        else
        {
            //errorTrigger = true;
            Debug.LogWarning("Formato no válido.");
        }

        // Si se ha detectado un error, volvemos a llamar a la función con el comando corregido por el GPT corrector.
        if (errorTrigger)
        {
            Debug.Log("Corrección: " + correction);
            errorTrigger = false;
            // Solo hago la llamada con el input corregido si no son idénticos, de esta forma evito entrar en bucle si la correción también es errónea
            if (inputString != correction)
            {
                InterpretString(correction);
            }
        }
    }

    // Función para hacer aparecer el prefab indicado como string
    private GameObject GetPrefabToSpawn(string objeto)
    {
        // Me aseguro de que el formato del nombre es el correcto
        string prefabName = char.ToUpper(objeto[0]) + objeto.Substring(1).ToLower();
        // Intento cargar el prefab desde la carpeta
        Debug.Log("El nombre del prefab a buscar será: " + prefabName);
        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + prefabName);
        if (prefab != null)
        {
            Debug.Log("El prefab encontrado es:" + prefab.name);
            // Si se encontró el prefab, lo devuelve
            return prefab;
        }
        else
        {
            // Si no se encontró el prefab, muestra una advertencia
            Debug.LogWarning("Prefab no encontrado con el nombre: " + prefabName);
            return null;
        }

    }

    private void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    private void NaeveDeath()
    {
        Debug.Log("Naeve ha muerto");
        naeveAnimator.SetTrigger("die");
        // Ponemos el juego a pausado para no seguir actualizandolo

        PauseMenu.gameIsPaused = true;
        Invoke("ActivatePause", 1.2f);
        // Abrimos el menú de muerte, desde el que se puede voler a empezar o salir con un poco de delay para poder ver la animación de muerte.
    }

    private void EndGame()
    {
        // Aquí iría el final del juego.
    }

    private void ActivatePause()
    {
        if (canvasObj != null)
        {
            PauseMenu pauseMenu = canvasObj.GetComponent<PauseMenu>();
            if (pauseMenu != null)
            {
                pauseMenu.Pause(deathMenuUI);
            }
        }
    }

    // Si hemos enviado un texto al GPT de manera manual, lo adquirimos el inputField de gptController y lo procesamos normalmente
    public async void ButtonPulsedAsync()
    {
        string answer = naeveControllerGPT.GetInputField();
        await SendAndHandleReply(answer);
    }

    public async void ButtonPulsedAsyncPorton()
    {
        string answer = portonControllerGPT.GetInputField();
        await SendAndHandleReplyNPC(portonControllerGPT, answer);
    }

    public async void ButtonPulsedAsyncEstatua()
    {
        string answer = estatuaControllerGPT.GetInputField();
        await SendAndHandleReplyNPC(estatuaControllerGPT, answer);
    }

    public async void ButtonPulsedAsyncQuimera()
    {
        string answer = quimeraControllerGPT.GetInputField();
        await SendAndHandleReplyNPC(quimeraControllerGPT, answer);
    }

    public async void ButtonPulsedAsyncLobo()
    {
        string answer = loboControllerGPT.GetInputField();
        await SendAndHandleReplyNPC(loboControllerGPT, answer);
    }

    public async void ButtonPulsedAsyncGuarda()
    {
        string answer = guardaControllerGPT.GetInputField();
        await SendAndHandleReplyNPC(guardaControllerGPT, answer);
    }

    private void FormatNaeveText()
    {
        Debug.Log("Texto antes del formateo: " + naeveText);

        string patron = @"\(\s*\d+(\.\d+)?\s*,\s*\d+(\.\d+)?\s*\)";

        // Utiliza Regex.Replace para reemplazar todos los números float entre paréntesis con una cadena vacía
        naeveText = Regex.Replace(naeveText, patron, "");

        // Imprime el texto resultante sin los números float entre paréntesis
        Debug.Log("Texto después del formateo: " + naeveText);

    }


}
