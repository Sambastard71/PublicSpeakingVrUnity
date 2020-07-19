import speech_recognition as sr
import pandas as pd
from gensim.models import Doc2Vec
from gensim.models.doc2vec import TaggedDocument
from nltk.tokenize import word_tokenize
from sklearn.linear_model import LogisticRegression
from stop_words import get_stop_words

'''
questi primi metodi sono di utility per suddividere calcoli ed operazioni e sono richiamati dai metodi principali
che trovi sotto nel codice. Un commento ti indicherà quando iniziano e ci saranno altri commenti per capire come usarli
'''

def preprocess(doc, stop_words):
    doc = doc.lower()  # Lower the text.
    doc = word_tokenize(doc)  # Split into words.
    doc = [w for w in doc if not w in stop_words]  # Remove stopwords.
    doc = [w for w in doc if w.isalpha()]  # Remove numbers and punctuation.
    return doc


def vec_for_learning(model, tagged_docs):
    sents = tagged_docs.values
    targets, regressors = zip(*[(doc.tags[0], model.infer_vector(doc.words, steps=20)) for doc in sents])
    return targets, regressors


def vec_for_classification(model, sentence, stop_words):
    s = preprocess(sentence, stop_words)
    regressors = [model.infer_vector(s, steps=20)]
    return regressors


def analyse_text(model, sentence, stop_words, csv_path):
    print("sto elaborando la seguente frase:")
    print("-> " + sentence)

    df = pd.read_csv(csv_path)
    df = df[['risposta', 'classe']]
    corpus = df.apply(lambda r: TaggedDocument(words=preprocess(r['risposta'], stop_words), tags=[r.classe]), axis=1)

    y_train, X_train = vec_for_learning(model, corpus)
    logreg = LogisticRegression(n_jobs=1, C=1e5)
    logreg.fit(X_train, y_train)

    vec_sentence = vec_for_classification(model, sentence, stop_words)
    pred = logreg.predict(vec_sentence)
    perc_pred = logreg.predict_proba(vec_sentence)
    prob = max(perc_pred[0])

    print("Classificato come: " + str(pred[0]) + ", con probabilità: " + str(prob))

    id_response = 70  # id di default
    if prob >= 0.5:
        id_response = pred[0]

    return id_response


def lesson_tree(model, sentence, stop_words, id_section, csv_path):
    """
    questa funzione decide la logica della lezione e come gestire i comandi che un utente può dare

    codici risposte:
    1 - Esegui Intro
    2 - Ripeti modulo
    3 - Avanti al prossimo modulo
    70 - utente ha detto una frase non legata ad una opzione
    80 - utente vuole uscire

    codici comandi in base alle risposte:
    1 - esegui Introduzione (solo inizio programma)
    2 - Ripeti questa lezione
    3 - Procedi a successiva
    70 - Opzione non presente, quindi ripete frase utente e dice che non è presente tra le scelte
    80 - Esci dal corso - Gestista al di fuori di questa funzione
    404 - Frase non compresa (errore nel convertire text2speech) - Gestista al di fuori di questa funzione

    i codici vengono trattati separatamente in quanto un utente può fare una determinata richiesta
    che potrebbe non essere consentita per l'attuale parte della lezione
    quindi quella richiesta con un determinato id sarà gestita con un comando apposito
    inoltre questo sistema predispone già scenari a selezione multipla o scenario a grafo
    """
    next_section = -1
    command = -1

    id_resp = analyse_text(model, sentence, stop_words, csv_path)

    if id_resp == 80:
        command = 80
        next_section = 0
    else:
        if id_resp == 70:
            command = 70
            next_section = id_section

        if id_resp == 1 and id_section == 0:
            command = 1
            next_section = 1

        if id_resp == 2 and 1 <= id_section < 6:
            command = 2
            next_section = id_section

        if id_resp == 3 and 1 <= id_section < 5:
            command = 3
            next_section = id_section + 1

        if id_resp == 3 and id_section == 5:
            command = 101
            next_section = 80

    return command, next_section


def execute_command(command, id_section):
    print("esecuzione di command: " + str(command) + " id lezione: " + str(id_section))
    if id_section == 0 and command != 1 and command < 70:
        print("Caro Utente, per iniziare ti ricordo che devi chiedere chiaramente di iniziare la lezione.")
    else:
        if command < 70:
            fileindex = str(id_section)
        else:
            fileindex = str(command)
        return fileindex

'''
Da qui in poi iniziano i metodi che ti servono in unity
'''

#funzione da chiamare inizialmente per fare il setup
def setup(path_modello):
    recognizer = sr.Recognizer()
    microphone = sr.Microphone()

    model = Doc2Vec.load(path_modello + "/doc2vec_model_04_02_2020")
    stop_words = get_stop_words('italian')


    return model, stop_words, recognizer, microphone


#funzione per ascoltare cosa dice l'utente
def runListener(microphone, recognizer):
    status_sentence = -1
    sentence = ""
    with microphone as source:
        recognizer.adjust_for_ambient_noise(source)
        audio = recognizer.listen(source)
    try:
        sentence = recognizer.recognize_google(audio, language="it-IT")
        status_sentence = 1
    except:
        sentence = ""
        status_sentence = -1

    return sentence, status_sentence

#se status_sentence è -1 all'ora l'avatar dovrà dire all'utente che quello che ha detto non si capisce,
#quindi dovrà eseguire il comando 404, ovvero il file 404.mp3


#funzione per elaborare cosa ha detto l'utente
def runSpeaker(model, sentence, id_section, stop_words, csv_path):
    command, id_section = lesson_tree(model, sentence, stop_words, id_section, csv_path)
    execute_command(command, id_section)
    return command, id_section

'''
se il comando è 80, ovvero 80.mp3, dopo aver riprodotto l'audio dovrà essere chiuso tutto

ora non ci sono più i thread ed i lock sui thread per decidere chi fa cosa e chi aspetta,
quindi prima si esegue il listener, si attende che c'è la risposta. Se la risposta è -1 si dice che non si è capito e si 
fa ripetere la frase, se invece è 1 si analizza la sentence mandandola a runSpeaker
per ogni dubbio scrivimi.

i path li passerai the ai metodi, così li puoi settare in unity comodamente. io ti dico solo quale mp3 utilizzare
poi il path completo lo farai da unity, ovvero c#
'''