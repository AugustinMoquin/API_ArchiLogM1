
# ArchiLog Rendu Final

Ce git repository contien le rendu du projet final pour la matière ArchiLog de :

    - Augustin Moquin
    - Cheikh Chouiref

Ce projet à pour objectif de fournir des informations sur le taux de change entre les différentes monnaies du monde
## Installation

### Creer la bdd

Faire ces commande dans le cmd de gestion de paquet NuGet
```bash
    Add-Migration <yourMigration>
    Update-DataBase
```
### Mettre à jour la bdd
Suivre la route
```bash
    /api/Import/import-currencies
```
## Utilisation
Lancer la solution en https
#### Exemple
```
https://localhost:7117/api/v1/Currency/compare?Currency_Code1=EUR&Currency_Code2=JPY
```
```JSON
{
  "currency1": {
    "id": 5,
    "country_Code": "AND",
    "country_number": 20,
    "country": "ANDORRA",
    "currency_Name": "Euro",
    "currency_Code": "EUR",
    "currency_Number": 978,
    "value": 0.942866
  },
  "currency2": {
    "id": 112,
    "country_Code": "JPN",
    "country_number": 392,
    "country": "JAPAN",
    "currency_Name": "Yen",
    "currency_Code": "JPY",
    "currency_Number": 392,
    "value": 149.83891
  },
  "comparison": {
    "EUR to JPY": {
      "EUR": 1,
      "JPY": 158.91856
    },
    "JPY to EUR": {
      "JPY": 1,
      "EUR": 0.0062925313
    }
  }
}
```
