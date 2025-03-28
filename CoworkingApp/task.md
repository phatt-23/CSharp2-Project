# C#2 Projekt (TRA0163)

Cílem projektu je návrh a implementace aplikace, která bude splňovat níže uvedené požadavky
a zároveň bude řešit evidenci v rámci definovaného tématu.

Projekt se bude skládat z webové a desktopové části. Součástí bude také řešení databáze,
která může být řešena pomocí libovolné relační DBMS (volba a způsob provozu je čistě na
volbě studenta). Jednotlivé funkční požadavky jednotlivých částí budou odpovídat
následujícímu:

## Webová aplikace

- Bude poskytovat API, které bude pokrývat kompletní funkcionalitu potřebnou pro
webovou i desktopovou část. Komunikace bude využívat formát JSON. Veškeré
implementace ve vazbě na API budou využívat asynchronní přístup.
- Bude zajišťovat přístup k databázi a práci s daty, a to s využitím dostupné vrstvy ORM
(např. Dapper).
- Bude nabízet komplexní funkcionalitu pro zajištění funkčnosti dle odpovídajícího tématu,
viz níže.
- Bude implementována problematika registrace uživatelů a jejich přihlašování. Typy
uživatelských rolí a konkrétní oprávnění budou minimálně „správce“ (přístup ke všemu)
a „jen pro čtení“ pouze nahlížení na data.
- Veškeré vstupní/editační formuláře budou implementovat funkcionalitu validace dat, a to
přinejmenším na straně serveru.
- Webová aplikace bude obsahovat funkcionalitu pro vizualizaci dat s využitím komponent
třetích stran např. grafy či mapa.
- Součást implementace bude API funkce (endpoint), která bude vracet popis rozhraní
API ve formátu JSON (inspirace v rámci Open API). K tomuto musí být využita reflexe.
Popis API musí obsahovat vše potřebné pro dokumentaci komunikace s tímto API
(adresa, HTTP metoda, parametry volání, popis návratových hodnot atd.).
- Aplikace bude vyvinuta v rámci platformy ASP.NET MVC.

## Desktopová aplikace

- Aplikace bude z pohledu funkčnosti poskytovat stejné možnosti jako webová aplikace
(nebude obsahovat vizualizaci dat formou grafů či mapy), a to v roli správce. Není třeba
řešit přihlašování, registraci a uživatelské role.
- Aplikace nebude přímo přistupovat k databázi, ale bude využívat API vyvinuté v rámci
webové části aplikace. Veškeré implementace ve vazbě na API budou využívat
asynchronní přístup.
- Aplikace bude vyvinuta v rámci platformy WPF příp. MAUI.
- Aplikace bude u všech formulářů řešit odpovídající validaci zadaných hodnot.


Všechna uvedená témata budou pokrývat následující funkcionalitu:
- Komplexní správa evidence všech informací (vytvoření, editace, mazání).
- Přehled evidovaných dat ve formě „gridů“ s možností řazení.
- Zobrazení detailu daného objektu.
- Dialog pro zajištění procesu evidence (využití nabíjení, zapůjčení kola, rezervace
pracovního místa apod.) musí zohledňovat stav daného objektu, tj. pokud je např.
konkrétní kolo zapůjčeno, nelze vytvořit novou zápůjčku, ale dialog nabídne funkcionalitu
vrácení. Toto musí být odpovídajícím způsobem vizualizováno uživateli vč. zdůvodnění.
Další poznámky k vývoji projektu:
- Součástí řešení bude i návrh databáze, která zajistí funkcionalitu požadovanou konkrétní
evidencí. Pokud je potřeba doplňte si další atributy např. pro identifikaci objektů.


# Evidence coworkingových prostor

## Evidované informace:
- coworkingový prostor vč. GPS souřadnic
- jednotlivá pracovní místa (jeden prostor může obsahovat N pracovních míst)
- stav pracovního místa (dostupné, obsazené, v údržbě) a historie změn stavů v čase
- obsazení pracovního místa (email zákazníka, čas začátku a konce, délka, cena).
 
## Specifická funkcionalita:
- zobrazení coworkingových center na mapě včetně aktuální dostupnosti pracovních míst
(webová část)
- možnost provést obsazení konkrétního místa (speciální stránka/dialog pro zaznamenání
obsazení a jejího průběhu)
- stav obsazení lze měnit přímo pouze pokud je "dostupné" a "v údržbě", nelze měnit stav
bez vazby na proces obsazení
- již ukončené obsazení nelze zpětně upravovat ani mazat (kontrola omezení jak v UI, tak
na straně dat)
- historie stavů pracovních míst se zaznamenává automaticky při změně aktuálního stavu,
není nutná plná editace, pouze náhled na data
- zobrazení statistiky, která ukáže počet ukončených využití pracovních míst za poslední
měsíc (příp. volitelně) pro jednotlivá coworkingová centra