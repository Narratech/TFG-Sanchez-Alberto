# TFG-Sanchez-Alberto

El repositorio creado inicialmente por Alberto está aquí: https://github.com/AlbertoSanchezMontalvo/TFG/tree/main, y contiene todo lo hecho hasta el 05/03/24

Manual de instalación y uso:

To use the OpenAI API, you need to have an OpenAI account. Follow these steps to create an account and generate an API key:

Go to https://openai.com/api and sign up for an account
Once you have created an account, go to https://beta.openai.com/account/api-keys
Create a new secret key and save it
Saving Your Credentials
To make requests to the OpenAI API, you need to use your API key and organization name (if applicable). To avoid exposing your API key in your Unity project, you can save it in your device's local storage.

To do this, follow these steps:

Create a folder called .openai in your home directory (e.g. C:User\UserName\ for Windows or ~\ for Linux or Mac)
Create a file called auth.json in the .openai folder
Add an api_key field and a organization field (if applicable) to the auth.json file and save it
Here is an example of what your auth.json file should look like:
{
    "api_key": "sk-...W6yi",
    "organization": "org-...L7W"
}
You can also pass your API key into OpenAIApi ctor when creating an instance of it but again, this is not recommended!

var openai = new OpenAIApi("sk-Me8...6yi");
IMPORTANT: Your API key is a secret. Do not share it with others or expose it in any client-side code (e.g. browsers, apps). If you are using OpenAI for production, make sure to run it on the server side, where your API key can be securely loaded from an environment variable or key management service.

El repositorio es un proyecto de Unity, por lo que bastará con importarlo en Unity y ejecutarlo.

Este TFG es una herramienta en forma de videojuego narrativo cuyos comportamientos están ligados a ChatGPT mediante una comunicación a través de la API.

Se podrá cambiar la versión de ChatGPT en el script "GPTController", dentro de la función "SendReply()", modificando la opción "Model". Justo debajo podrá modificarse también la temperatura, un número entre 0.0f y 1.0f. Cuanta más temperatura más creatividad del modelo, aunque más tendencia a la alucinación. Según el ejemplar de ChatGPT en el que se configure se necesitará una temperatura más alta o más baja. Los parámetros por defecto se encuentran en la función "Start()": 

private void Start()
        {
            if (this.name == "Action GPT")
            {
                temperature = 0.5f;
            }
            else
            {
                temperature = 0.7f;
            }
        }

En la versión por defecto se trata la historia de una niña, Naeve, que ha perdido a sus padres y por debido al shock el juego transcurre en su propia mente, y así han sido configurados los diferentes prompts. En esta versión hay dos NPCs preparados en la escena: una puerta parlante a la que el jugador tendrá que convencer de que le deje pasar y una estatua que puede dar un objeto al jugador. Hay otros NPCs implementados pero desactivados de inicio; son una quimera, que te permite el paso si aciertas su adivinanza; un lobo, que te seguirá si eres capaz de convencerlo; y un guardabosques, que dará la alarma o se irá a su cabaña en función de las respuesta del jugador. Cada uno tiene su propio prompt y se tendrá que interactuar con él por medio de un chat de texto. Estos NPCs se pueden añadir, así como quitar los actuales, modificando la escena y el código como se indica a continuación: 

La función "Start()" de la clase "ResponseTraduction" tiene las siguientes llamadas a funciones que inicializan cada NPC:

InitializePorton();
InitializeEstatua();
//InitializeQuimera();
//InitializeLobo();
//InitializeGuarda();

Se podrá comentar y descomentar la inicialización que se desee. Además, se ha de activar el NPC en la escena, dentro de la jerarquía de objetos, en "Level", en "Layer-2" se encuentran todos los NPCs y personajes.

Se recomienda, si se van a probar otros NPCs o no se quiere jugar la historia por defecto, desactivar la variable estática "activeEnemy" dentro del script "EnemyController":

public static bool activeEnemy = false;

Además, se ha de tener en cuenta que la únicas acciones implementadas en la herramienta son:

Coger - Coge el objeto indicado.
Mover - Mueve a Naeve a la posición indicada.
Desaparecer - Hace desaparecer un objeto o un ente de la escena. 
Menguar - Hace menguar un objeto o ente. 
Crecer - Hace crecer un objeto o ente. 
Explotar - Hace explotar un objeto o ente. 
Atacar - Ataca a un ente o un objeto. 
Esconderse - Naeve se esconde detrás del objeto indicado.
Atraer - Atrae un objeto o ente hacia Naeve.
Teletransportar - Teletransporta un ente u objeto a la posición indicada.
Soltar - Suelta un objeto del inventario.
Levitar - Hace levitar un ente u objeto.
Materializar - Hace aparecer un objeto inexistente pero creable.
Utilizar - Utiliza un objeto del inventario.
Saltar - Naeve salta hacia delante.
Hablar - Naeve habla con un ente.
Esperar - Naeve aguarda temerosa a la siguiente acción.
Caer - Hacer caer un objeto en la altura al suelo.
Invisibilizar - Hace invisible un ente u objeto.
Controlar - El jugador toma el control de Naeve.

Como recomendaciones generales, el mejor modo de moverse es haciendo click al lugar del escenario al que se desea ir. Todo lo demás es mejor y hacerlo a través del Chat. Por cómo es el prompt del personaje principal (que también se podría modificar) es mejor que, al hablar con el ChatGPT de la protagonista, se interactúe como si se estuviese narrando la historia en tercera persona. Por ejemplo, si quieres hacer que el cómic que ha en el árbol se caiga puedes decir: "Naeve agitaría las ramas del árbol para intentar que el cómic cayese". Otro dato importante es que uno de los ejemplares de ChatGPT interpreta posiciones x e y para mover al personaje, por lo que se puede mencionar la posición a la que se desea mover o teletransportar a Naeve. Un ejemplo interesante de lo que se puede hacer es decirle algo como: "Naeve se sentiría tan poderosa al estar dentro de su propia mente que a partir de ahora siempre que vaya a moverse, se teltransportaría en su lugar, con un poder que ha sido capaz de canalizar", con el objetivo de teletransportarse en vez de moverse cada vez que se hace click en el escenario.

