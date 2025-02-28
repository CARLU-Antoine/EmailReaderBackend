# EmailReaderBackend

EmailReaderBackend est une application backend développée en C# utilisant **Microsoft Graph API** pour récupérer les emails d'un compte utilisateur Microsoft. Ce projet permet de s'authentifier via **OAuth2.0** en utilisant le flux d'authentification interactif ou silencieux, et de récupérer tous les emails d'une boîte de réception sans limite de 1000 emails.

## Fonctionnalités

- **Authentification via Microsoft Identity Platform** : Le backend utilise **MSAL** (Microsoft Authentication Library) pour gérer l'authentification et récupérer un **jeton d'accès** afin d'interagir avec l'API Microsoft Graph.
- **Récupération des emails** : Utilisation de **Microsoft Graph API** pour accéder à la boîte de réception et récupérer les emails. Le code prend en charge la pagination pour récupérer tous les emails.

- **Barre de progression** : Affichage dynamique de la barre de progression pour indiquer l'avancement du téléchargement des emails.
- **JSON** : Les résultats sont convertis en format JSON pour être utilisés dans d'autres applications ou systèmes.

## Technologies

- **C# 9.0+**
- **Microsoft Graph API** : Pour interagir avec les emails via l'API de Microsoft.
- **MSAL (Microsoft Authentication Library)** : Pour gérer l'authentification avec Azure Active Directory.
- **Newtonsoft.Json** : Pour la sérialisation des données en format JSON.
- **HttpClient** : Pour effectuer des requêtes HTTP vers Microsoft Graph.

## Prérequis

- **.NET SDK** (version 5.0 ou supérieure)
- **Compte Microsoft** (pour accéder à Microsoft Graph API)
- **Application enregistrée sur Azure AD** avec les permissions `Mail.Read` et `User.Read`.

## Installation

1. **Cloner le repository** :

   ```bash
   git clone https://github.com/votre-utilisateur/EmailReaderBackend.git
   cd EmailReaderBackend
