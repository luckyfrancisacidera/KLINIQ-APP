# Symptom catalog review diff

Baseline: the original inline `ExplainableSymptomAnalysisService` catalog.

The checked-in catalog is a conservative, human-reviewed expansion informed by the public dataset schemas and sample records listed in the builder README. Raw third-party datasets are not redistributed.

| Specialty | Baseline | Reviewed catalog | Added |
|---|---:|---:|---:|
| Cardiology | 6 | 21 | 15 |
| Pulmonology | 6 | 21 | 15 |
| Dermatology | 7 | 27 | 20 |
| Neurology | 8 | 25 | 17 |
| Gastroenterology | 8 | 27 | 19 |
| Pediatrics | 6 | 18 | 12 |
| Obstetrics and Gynecology | 7 | 23 | 16 |
| Orthopedics | 8 | 26 | 18 |
| Otolaryngology (ENT) | 7 | 23 | 16 |
| Ophthalmology | 6 | 22 | 16 |
| Psychiatry or Psychology | 7 | 23 | 16 |
| Endocrinology | 5 | 20 | 15 |
| Urology | 6 | 22 | 16 |
| General Practice | 7 | 25 | 18 |

## Added phrases by specialty

### Cardiology

`ankle swelling`, `chest pain`, `cold sweats`, `exercise intolerance`, `fainting`, `heart fluttering`, `irregular heartbeat`, `lightheaded with palpitations`, `low blood pressure`, `pain radiating to arm`, `pain radiating to jaw`, `rapid heart rate`, `shortness of breath on exertion`, `slow heart rate`, `swollen legs`

### Pulmonology

`blue lips`, `breathlessness`, `chest tightness`, `coughing blood`, `coughing up mucus`, `difficulty breathing`, `low oxygen`, `night cough`, `noisy breathing`, `pain when breathing`, `rapid breathing`, `rusty phlegm`, `shallow breathing`, `snoring`, `trouble breathing`

### Dermatology

`blisters`, `dry skin`, `flaky skin`, `fluid filled blisters`, `hair loss`, `itching`, `nail changes`, `painful skin`, `pus from skin`, `red skin`, `scaly skin`, `silvery scales`, `skin bumps`, `skin discoloration`, `skin peeling`, `skin rash`, `skin redness`, `skin sores`, `skin swelling`, `slow healing wound`

### Neurology

`balance problems`, `burning pain`, `confusion`, `facial weakness`, `lightheadedness`, `loss of consciousness`, `loss of sensation`, `memory loss`, `muscle twitching`, `poor coordination`, `sensitivity to light`, `sensitivity to sound`, `shooting pain`, `speech difficulty`, `throbbing headache`, `vision aura`, `weakness on one side`

### Gastroenterology

`abdominal swelling`, `belching`, `black stool`, `bloating`, `blood in stool`, `dark urine`, `difficulty swallowing`, `food stuck in throat`, `hiccups`, `indigestion`, `jaundice`, `loss of appetite`, `lower abdominal pain`, `nausea`, `pale stool`, `rectal pain`, `sour taste`, `upper abdominal pain`, `yellow skin`

### Pediatrics

`baby fever`, `child fever`, `childhood illness`, `delayed growth`, `developmental delay`, `failure to gain weight`, `fussiness`, `infant cough`, `newborn jaundice`, `poor feeding`, `rash at birth`, `school age child`

### Obstetrics and Gynecology

`breast lump`, `fertility concern`, `heavy periods`, `irregular periods`, `missed period`, `morning sickness`, `painful intercourse`, `painful periods`, `postpartum bleeding`, `pregnancy bleeding`, `prenatal care`, `severe vomiting in pregnancy`, `vaginal bleeding`, `vaginal burning`, `vaginal discharge`, `vaginal dryness`

### Orthopedics

`ankle pain`, `bone deformity`, `bruising`, `difficulty moving`, `hand pain`, `hip pain`, `joint stiffness`, `joint swelling`, `limited range of motion`, `loss of function`, `lower back pain`, `muscle pain`, `muscle weakness`, `neck pain`, `pain when walking`, `radiating leg pain`, `shoulder stiffness`, `wrist pain`

### Otolaryngology (ENT)

`bad taste`, `difficulty swallowing`, `drooling`, `dry mouth`, `ear discharge`, `facial pressure`, `hoarseness`, `loss of smell`, `nosebleed`, `ringing in ears`, `runny nose`, `sinus pressure`, `stuffy nose`, `swollen salivary glands`, `throat swelling`, `voice changes`

### Ophthalmology

`cloudy eyes`, `dark spots in vision`, `distorted vision`, `double vision`, `dry eyes`, `excessive tearing`, `eye fatigue`, `floaters`, `halos around lights`, `hazy vision`, `impaired color vision`, `light sensitivity`, `peripheral vision loss`, `poor depth perception`, `tunnel vision`, `visual field loss`

### Psychiatry or Psychology

`binge eating`, `compulsive behavior`, `difficulty concentrating`, `distorted body image`, `fear of gaining weight`, `fear of losing control`, `hallucinations`, `irritability`, `mood changes`, `mood swings`, `obsessive thoughts`, `panic`, `purging`, `sleep disturbance`, `social withdrawal`, `unrefreshing sleep`

### Endocrinology

`cold intolerance`, `excessive hunger`, `excessive thirst`, `goiter`, `heat intolerance`, `high blood sugar in pregnancy`, `increased hunger`, `increased thirst`, `low blood sugar`, `neck swelling`, `recurrent infections`, `shaking from low sugar`, `slow healing cuts`, `unexpected weight gain`, `unexpected weight loss`

### Urology

`back or flank pain`, `bedwetting`, `burning urination`, `cloudy urine`, `decreased urine output`, `difficulty urinating`, `flank pain`, `foul smelling urine`, `incomplete bladder emptying`, `nighttime urination`, `scrotal swelling`, `strong urine odor`, `testicular pain`, `urinary urgency`, `urine leakage`, `weak urine stream`

### General Practice

`allergic reaction`, `body aches`, `chills`, `dehydration`, `general weakness`, `loss of appetite`, `malaise`, `medical checkup`, `medication reaction`, `muscle aches`, `night sweats`, `persistent tiredness`, `poor appetite`, `routine checkup`, `sneezing`, `sweating`, `swollen lymph nodes`, `unexplained fever`

## Unmapped diseases

No complete third-party export set was bundled with the repository. `unmapped-diseases.txt` is regenerated from the local exports every time the catalog builder runs. The builder never guesses a specialty for an unmapped disease.

## Review notes

- Generic one-word signals such as `pain` and `swelling` were intentionally not added to specialty catalogs because they create unsafe cross-specialty matches.
- Emergency and urgent phrases remain a separate, manually reviewed list in the runtime service.
- Filipino and Ilocano translations remain a follow-up requiring clinician-reviewed curation.
