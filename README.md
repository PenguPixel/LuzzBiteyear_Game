# Luzz Biteyear - Mission to Infinity

> "Dein Abenteuer im Wild West von morgen."
>
> Ein 3D-Action-Adventure im Space-Western-Setting, entstanden im Rahmen des PIP-Projekts (Toys'for'us). Begleite die Erkundungseinheit **Luzz Biteyear** auf einem rauen Wüstenplaneten, überwinde Hindernisse per Hacking-Plug-in und setze dich im Kampf gegen feindliche Einheiten durch.

## Ueber das Spiel

Im 3D-Action-Adventure **Luzz Biteyear** strandet die gleichnamige Roboter-Haupteinheit nach einer Bruchlandung neben dem Wrack des Raumschiffs auf einem unbarmherzigen Wüstenplaneten. Eigentlich mit dem Auftrag entsandt, neue Territorien für die menschliche Erschließung zu erforschen, findet sich Luzz nun isoliert in einer staubigen Canyon-Landschaft wieder.

Geleitet vom digitalen Begleiter **Albyte** (Narrator-Tablet) lernt Luzz Biteyear, sich im Terrain zu bewegen, Gefahren durch Nah- und Fernkampf abzuwehren und versperrte Wege zu öffnen.

## Kern-Features und USP

* **Der modulare Schnittstellen-USP ("Plug-in & Hack"):**
  Luzz Biteyear besitzt einen funktionalen Kabelschwanz mit Universalstecker. An versperrten Toren oder Sicherheitsterminals (`PlugTerminal`) dockt Luzz an, woraufhin das Spiel in den `GameState.Puzzle` wechselt. Durch das Lösen eines Schaltungs-Minispiels (*Circuit Connect*) werden Stromkreise repariert und Barrieren entriegelt.

* **Dynamisches Kampfsystem:**

  * **Fernkampf (`ShootingComponent`):** Laser-Projektile auf Knopfdruck, gekoppelt an die Energieressource.

  * **Nahkampf (`AttackComponent`):** Frontaler Biss-Angriff ("Chomp"), getriggert via Animation-Events (`AE_OnMeleeHitFrame`).

* **Gegner-Design & KI-Encounter:**
  Dynamische Spawns über Encounter-Trigger im Unity-NavMesh:

  * *Kaktus-Gegner:* Stationärer Fernkämpfer.

  * *Chomper:* Mobiler Nahkämpfer mit Verfolgung.

  * *Snake:* Mobiler Fernkämpfer mit Distanzkontrolle.

* **Ressourcen- und Ueberlebens-Loop:**

  * **Energie:** Dient als Munition für Blaster-Schüsse; regeneriert sich passiv oder durch Treffer.

  * **Lebenspunkte:** Nahkampf-Executes an Gegnern sowie das Zerstören von Umgebungselementen (Heil-Kakteen) stellen Trefferpunkte wieder her.

* **Target-Lock und Strafe-Movement:**
  Automatisches Aufschalten (`ITargetable`) in Reichweite und manuelles Durchschalten von Zielen (*Target Cycling*) per Taste (`Q` / `E`). Die Bewegungslogik passt sich bei aktivem Ziel mit einer Strafe-Ausrichtung an.

## Steuerung

| **Aktion** | **Tastatur & Maus** |
| **Bewegung** | `W` `A` `S` `D` |
| **Kamera / Blick** | Maus |
| **Springen** | `Leertaste` |
| **Interaktion / Plug-in** | `F` |
| **Ziel durchschalten** | `Q` / `E` | 
| **Fernkampf (Schiessen)** | `Linksklick (Maus)` |
| **Nahkampf (Chomp)** | `Rechtsklick (Maus)` | 
| **Pause / Menue** | `Escape` |

## Software- und Systemarchitektur

Die Codebase folgt konsequent dem **Single Responsibility Principle (SRP)** sowie einer entkoppelten, eventbasierten Architektur:

* **Zustandsverwaltung (`GameManager` & `GameStateVariable`):**
  Verwaltet globale Spielzustände (`Exploration`, `Combat`, `Puzzle`, `Paused`, `GameOver`) über ScriptableObjects und benachrichtigt Systeme via `GameEvent`.

* **Entkoppelte Input-Verarbeitung:**

  * `InputComponent`: Liest reine Eingaben über das Unity Input System aus.

  * `PlayerComponent` & `InteractionComponent`: Verarbeiten den Kontext und steuern Logikkomponenten an.

  * `MovementComponent`: Führt die physikbasierte Fortbewegung via Rigidbody im `FixedUpdate` aus.

* **Single-Scene UI-Management (`UIManager`):**
  Das Umschalten sichtbarer Panels (`HUDPanel`, `PuzzlePanel`, `MenuPanel`, `GameOverPanel`) erfolgt zustandsgesteuert im Canvas, ohne additive Szenen-Ladezeiten zu erzwingen.

* **Wiederverwendbare Schnittstellen (Interfaces):**

  * `IInteractable`: Welt-Interaktionen mit Bereitstellung eines `InteractionData`-ScriptableObjects.

  * `ITargetable`: Einheitliche Schnittstelle zur Zielerfassung für Gegner und Schalter.

  * `IDamageable`: Schadensabwicklung für Spieler, KI-Gegner und zerstörbare Umweltobjekte (z. B. Kakteen).

  * `IUseEnergy` / `IReplenishEnergy`: Regelung des Energiehaushalts für Blaster und Aktionen.

* **Entkoppelte Animationen (`CharacterAnimationBridge` / `DoorAnimationBridge`):**
  Gameplay-Klassen kommunizieren mit Unitys `Animator` ausschließlich über gehashte Parameter (`Animator.StringToHash`) und empfangen Signale über Animation-Events.

## Credits und Team

Entstanden im Rahmen des Moduls **PIP** (Projektarbeit):

* **Development (GME):**

  * **Philipp Locher** (pengupixels.de) - Lead Dev: Character Locomotion, Input Decoupling, Puzzle/Terminal Integration, Scene Architecture

  * **Christof Kloninger** - Core State Management, Interfaces, Combat/Audio Architecture, Targeting

* **3D Art & Modeling (3DE):**

  * Alex, Jonathan, Kuri

* **Cross Media Design (XMD):**

  * Jennifer Mohr, Marianna Batkova
