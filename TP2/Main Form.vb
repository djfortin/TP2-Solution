'Main Form.vb
'TP2 - Jeux MasterMind
'2020-03-12
'111245796

Option Explicit On
Option Strict On
Option Infer Off

Public Class frmMain

    Private intCouleurSelect As Integer = 1
    Private intCompteurTour As Integer = 1
    Private intReponse(3) As Integer
    Private intChoixCouleur(3) As Integer
    Private intNbCoupExact As Integer
    Private intNbCoupManque As Integer = 0

    ' Procédure Sub et Function indépendantes.

    Private Sub InitialiseReponse()
        ' Initialise les 4 couleurs aléatoires lors de la partie en cours.

        Dim intNum As Integer
        Static randGen As New Random

        ' Copie les couleurs aléatoires dans une liste.
        For intI As Integer = 1 To 4
            intNum = randGen.Next(1, 7)
            intReponse(intI - 1) = intNum
        Next intI

    End Sub

    Private Sub InitialiserEtoileCouleur()
        ' Enlève toutes les étoiles sur les couleurs sélectionnées.

        ' Boucle à travers toutes les étiquettes.
        For intI As Integer = 1 To 6
            Me.Controls("lblCouleur" & intI).Text = intI.ToString
        Next intI

    End Sub

    Private Sub ActiveCouleur(ByVal intNoCouleur As Integer)
        ' Active la couleur selon le numéro de 1 à 6 choisi par le joueur.

        ' Enlève toutes les étoiles(*).
        InitialiserEtoileCouleur()

        ' Sélectionne la couleur avec l'étoile(*).
        intCouleurSelect = intNoCouleur
        Me.Controls("lblCouleur" & intNoCouleur).Text = intNoCouleur & "*"

    End Sub

    Private Sub ColorieCaseChoisie(ByVal intNoCase As Integer)
        ' Colore la case choisie par le joueur selon A, B, C ou D.

        ' Colorie le Label choisi.
        Me.Controls("grpCol" & intCompteurTour).
            Controls("lbl" & intCompteurTour & intNoCase).BackColor = SelectionneCouleur(intCouleurSelect)

        ' Ajoute la couleur dans la liste des couleurs choisies par le joueur.
        SelectionneCouleurColonne(Me.Controls("grpCol" & intCompteurTour).
                                  Controls("lbl" & intCompteurTour & intNoCase).Name.Substring(4))

        ' Active le bouton Vérifier s'il y a lieu.
        ActiveVerification()

    End Sub

    Private Sub SelectionneCouleurColonne(ByVal strNoRangee As String)
        ' Place la couleur sélectionnée dans une liste qui correspond au choix du joueur.

        Dim intNoRangee As Integer

        Integer.TryParse(strNoRangee, intNoRangee)
        intChoixCouleur(intNoRangee - 1) = intCouleurSelect

    End Sub

    Private Sub ValiderCoup()
        'Valide et compare le coup du joueur avec la bonne réponse.

        Dim blnCouleurMatrice(,) As Boolean = {{False, False},
                                               {False, False},
                                               {False, False},
                                               {False, False}}

        ' Vérifie les coups exacts.
        For intI As Integer = 0 To intChoixCouleur.GetUpperBound(0) 'Boucle les choix du joueur.
            For intJ As Integer = 0 To intReponse.GetUpperBound(0) 'Boucle les réponses.
                ' Si couleur identique.
                If intChoixCouleur(intI) = intReponse(intJ) Then
                    ' Si l'index est identique.
                    If intI = intJ Then
                        CoupBonExact()
                        ' Ajoute True dans la matrice.
                        blnCouleurMatrice(intI, 0) = True
                        blnCouleurMatrice(intJ, 1) = True
                        Exit For
                    End If
                End If
            Next intJ
        Next intI

        ' Vérifie les coups manqués.
        For intI As Integer = 0 To intChoixCouleur.GetUpperBound(0) 'Boucle les choix du joueur.
            For intJ As Integer = 0 To intReponse.GetUpperBound(0) 'Boucle les réponses.
                ' Si couleur identique et que la matrice est False.
                If intChoixCouleur(intI) = intReponse(intJ) AndAlso blnCouleurMatrice(intI, 0) = False AndAlso
                    blnCouleurMatrice(intJ, 1) = False Then
                    CoupBonManque()
                    ' Ajoute True dans la matrice.
                    blnCouleurMatrice(intI, 0) = True
                    blnCouleurMatrice(intJ, 1) = True
                    Exit For
                End If
            Next intJ
        Next intI

        ' Vérifie le status de la partie et, s'il y a lieu, incrémente la prochaine colonne.
        If PartieGagnee() Then
            MessageBox.Show("Vous avez gagné la partie!", "Victoria!",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            AfficherReponse()
            btnVerifier.Enabled = False
            mnuFichierVerifier.Enabled = False
        ElseIf PartiePerdue() Then
            MessageBox.Show("Vous avez perdu la partie!", "Cladem!",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            AfficherReponse()
            btnVerifier.Enabled = False
            mnuFichierVerifier.Enabled = False
        Else
            MarquerCoupBlanc(intCompteurTour)
            IncrementeCoup()
        End If

    End Sub

    Private Sub IncrementeCoup()
        ' Incrémente de 1 le nombre de coup possible et active la colonne suivante.

        intCompteurTour += 1
        intNbCoupExact = 0
        intNbCoupManque = 0

        Me.Controls("grpCol" & intCompteurTour).Enabled = True

        ' Désactive le bouton de vérification.
        btnVerifier.Enabled = False
        mnuFichierVerifier.Enabled = False

        ' Vide la liste des choix du joueur.
        ReInitialiseChoixCouleur()

    End Sub

    Private Sub CoupBonExact()
        ' Marque les coups qui sont bon et se trouve à la bonne place.

        intNbCoupExact += 1

        ' Ajout marqueur noir.
        MarquerCoupNoir(intCompteurTour)

    End Sub

    Private Sub CoupBonManque()
        ' Marque les coups qui sont bon et se trouve à la mauvaise place.

        intNbCoupManque += 1

    End Sub

    Private Sub MarquerCoupNoir(ByVal intNoColonne As Integer)
        ' Marque les bon coups qui sont exact par des jetons noirs.

        ' Boucle et colorie en noir les bonne réponses.
        For intI As Integer = 1 To 4
            ' Si la case est vide, colorie en noir.
            If Me.Controls("grpCol" & intNoColonne).Controls("lblVerif" & intNoColonne & intI).BackColor <> Color.Black Then
                Me.Controls("grpCol" & intNoColonne).Controls("lblVerif" & intNoColonne & intI).BackColor = Color.Black
                Exit Sub
            End If
        Next intI

    End Sub

    Private Sub MarquerCoupBlanc(ByVal intNoColonne As Integer)
        ' Marque les bon coups qui sont exact par des jetons blancs.

        ' Boucle et colorie en blanc les réponses selon le nombre de coups manqués.
        For intI As Integer = 1 To intNbCoupManque
            For intJ As Integer = 1 To 4
                ' Si la case est vide, colorie en blanc.
                If Me.Controls("grpCol" & intNoColonne).Controls("lblVerif" & intNoColonne & intJ).BackColor <> Color.Black AndAlso
                    Me.Controls("grpCol" & intNoColonne).Controls("lblVerif" & intNoColonne & intJ).BackColor <> Color.White Then
                    Me.Controls("grpCol" & intNoColonne).Controls("lblVerif" & intNoColonne & intJ).BackColor = Color.White
                    Exit For
                End If
            Next intJ
        Next intI

    End Sub

    Private Sub ActiveVerification()
        ' Vérifie si le boutton Vérifier peut être activer.

        Dim intCompteur As Integer = 0

        ' Incrémente pour chaque item si la valeur est plus grande que zéro.
        For Each intItem As Integer In intChoixCouleur
            If intItem > 0 Then
                intCompteur += 1
            End If
        Next

        ' Active le bouton si il y a 4 couleurs de sélectionnées.
        If intCompteur = 4 Then
            btnVerifier.Enabled = True
            btnVerifier.Select()
            mnuFichierVerifier.Enabled = True
        End If

    End Sub

    Private Sub ReInitialiseChoixCouleur()
        ' Remise à zéro de la liste des choix de couleur.

        For intI As Integer = 0 To 3
            intChoixCouleur(intI) = 0
        Next

    End Sub

    Private Sub RecommencerPartie()
        ' Initialise la partie au début.

        intCompteurTour = 1
        intNbCoupExact = 0
        intNbCoupManque = 0

        grpCol1.Enabled = True

        ' Désactive tous les GroupBox de 2 à 8.
        For intI As Integer = 2 To 8
            Me.Controls("grpCol" & intI).Enabled = False
        Next intI

        ' Enlève toutes les couleurs sur la table de jeu.
        For intI As Integer = 1 To 8
            For intJ As Integer = 1 To 4
                Me.Controls("grpCol" & intI).Controls("lbl" & intI & intJ).BackColor = Color.Empty
                Me.Controls("grpCol" & intI).Controls("lblVerif" & intI & intJ).BackColor = Color.Empty
                grpReponse.Controls("lblRep" & intJ).BackColor = Color.Empty
            Next intJ
        Next intI

        ' Charge une réponse aléatoire.
        InitialiseReponse()

        ' Enlève toutes les étoiles.
        InitialiserEtoileCouleur()

        ' Sélectionne la couleur par défaut.
        intCouleurSelect = 1
        lblCouleur1.Text = "1*"

        ' Désactive le bouton de vérification.
        btnVerifier.Enabled = False

        ' Remet à zéro les choix de couleur.
        ReInitialiseChoixCouleur()

    End Sub

    Private Sub AfficherReponse()
        ' Affiche la réponse en couleur dans le formulaire.

        ' Boucle la liste des réponses aléatoires et colorie la case de la bonne couleur.
        For intI As Integer = 1 To intReponse.Length
            grpReponse.Controls("lblRep" & intI).BackColor = SelectionneCouleur(intReponse(intI - 1))
        Next intI

    End Sub

    Function SelectionneCouleur(ByVal intNoCouleur As Integer) As Color
        ' Selon le numéro de la couleur choisie par le joueur, renvoie un object de type color.
        ' Retour: Un objet de type Color.

        Select Case intNoCouleur
            Case 1
                Return Color.White
            Case 2
                Return Color.Purple
            Case 3
                Return Color.Yellow
            Case 4
                Return Color.Green
            Case 5
                Return Color.Red
            Case 6
                Return Color.Blue
        End Select

    End Function

    Function PartieGagnee() As Boolean
        ' Vérifie si la partie est une partie gagnante. La partie est gagnante lorsqu'il y 4 couleurs identiques.
        ' Retour: True si elle est gagnante, False sinon.

        If intNbCoupExact = 4 Then
            Return True
        Else
            Return False
        End If

    End Function

    Function PartiePerdue() As Boolean
        ' Vérifie si la partie est une partie perdue. Lorsque le compteur atteint 9 tours, la partie est perdue.
        ' Retour: True si elle est perdue, False sinon.

        If intCompteurTour >= 8 Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Sub btnQuitter_Click(sender As Object, e As EventArgs) Handles btnQuitter.Click

        Me.Close()

    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' Prépare la première initialisation du jeu.

        InitialiseReponse()

    End Sub

    Private Sub lblCouleur1_Click(sender As Object, e As EventArgs) Handles lblCouleur1.Click
        ' Sélectionne la couleur 1

        ActiveCouleur(1)

    End Sub

    Private Sub lblCouleur2_Click(sender As Object, e As EventArgs) Handles lblCouleur2.Click
        ' Sélectionne la couleur 2

        ActiveCouleur(2)

    End Sub

    Private Sub lblCouleur3_Click(sender As Object, e As EventArgs) Handles lblCouleur3.Click
        ' Sélectionne la couleur 3

        ActiveCouleur(3)

    End Sub

    Private Sub lblCouleur4_Click(sender As Object, e As EventArgs) Handles lblCouleur4.Click
        ' Sélectionne la couleur 4

        ActiveCouleur(4)

    End Sub

    Private Sub lblCouleur5_Click(sender As Object, e As EventArgs) Handles lblCouleur5.Click
        ' Sélectionne la couleur 5

        ActiveCouleur(5)

    End Sub

    Private Sub lblCouleur6_Click(sender As Object, e As EventArgs) Handles lblCouleur6.Click
        ' Sélectionne la couleur 6

        ActiveCouleur(6)

    End Sub

    Private Sub lbl11_Click(sender As Object, e As EventArgs) Handles lbl11.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 1 Then
            ColorieCaseChoisie(1)
        End If

    End Sub

    Private Sub lbl12_Click(sender As Object, e As EventArgs) Handles lbl12.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 1 Then
            ColorieCaseChoisie(2)
        End If

    End Sub

    Private Sub lbl13_Click(sender As Object, e As EventArgs) Handles lbl13.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 1 Then
            ColorieCaseChoisie(3)
        End If

    End Sub

    Private Sub lbl14_Click(sender As Object, e As EventArgs) Handles lbl14.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 1 Then
            ColorieCaseChoisie(4)
        End If

    End Sub

    Private Sub lbl21_Click(sender As Object, e As EventArgs) Handles lbl21.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 2 Then
            ColorieCaseChoisie(1)
        End If

    End Sub

    Private Sub lbl22_Click(sender As Object, e As EventArgs) Handles lbl22.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 2 Then
            ColorieCaseChoisie(2)
        End If

    End Sub

    Private Sub lbl23_Click(sender As Object, e As EventArgs) Handles lbl23.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 2 Then
            ColorieCaseChoisie(3)
        End If

    End Sub

    Private Sub lbl24_Click(sender As Object, e As EventArgs) Handles lbl24.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 2 Then
            ColorieCaseChoisie(4)
        End If

    End Sub

    Private Sub lbl31_Click(sender As Object, e As EventArgs) Handles lbl31.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 3 Then
            ColorieCaseChoisie(1)
        End If

    End Sub

    Private Sub lbl32_Click(sender As Object, e As EventArgs) Handles lbl32.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 3 Then
            ColorieCaseChoisie(2)
        End If

    End Sub

    Private Sub lbl33_Click(sender As Object, e As EventArgs) Handles lbl33.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 3 Then
            ColorieCaseChoisie(3)
        End If

    End Sub

    Private Sub lbl34_Click(sender As Object, e As EventArgs) Handles lbl34.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 3 Then
            ColorieCaseChoisie(4)
        End If

    End Sub

    Private Sub lbl41_Click(sender As Object, e As EventArgs) Handles lbl41.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 4 Then
            ColorieCaseChoisie(1)
        End If

    End Sub

    Private Sub lbl42_Click(sender As Object, e As EventArgs) Handles lbl42.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 4 Then
            ColorieCaseChoisie(2)
        End If

    End Sub

    Private Sub lbl43_Click(sender As Object, e As EventArgs) Handles lbl43.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 4 Then
            ColorieCaseChoisie(3)
        End If

    End Sub

    Private Sub lbl44_Click(sender As Object, e As EventArgs) Handles lbl44.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 4 Then
            ColorieCaseChoisie(4)
        End If

    End Sub

    Private Sub lbl51_Click(sender As Object, e As EventArgs) Handles lbl51.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 5 Then
            ColorieCaseChoisie(1)
        End If

    End Sub

    Private Sub lbl52_Click(sender As Object, e As EventArgs) Handles lbl52.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 5 Then
            ColorieCaseChoisie(2)
        End If

    End Sub

    Private Sub lbl53_Click(sender As Object, e As EventArgs) Handles lbl53.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 5 Then
            ColorieCaseChoisie(3)
        End If

    End Sub

    Private Sub lbl54_Click(sender As Object, e As EventArgs) Handles lbl54.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 5 Then
            ColorieCaseChoisie(4)
        End If

    End Sub

    Private Sub lbl61_Click(sender As Object, e As EventArgs) Handles lbl61.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 6 Then
            ColorieCaseChoisie(1)
        End If

    End Sub

    Private Sub lbl62_Click(sender As Object, e As EventArgs) Handles lbl62.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 6 Then
            ColorieCaseChoisie(2)
        End If

    End Sub

    Private Sub lbl63_Click(sender As Object, e As EventArgs) Handles lbl63.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 6 Then
            ColorieCaseChoisie(3)
        End If

    End Sub

    Private Sub lbl64_Click(sender As Object, e As EventArgs) Handles lbl64.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 6 Then
            ColorieCaseChoisie(4)
        End If

    End Sub

    Private Sub lbl71_Click(sender As Object, e As EventArgs) Handles lbl71.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 7 Then
            ColorieCaseChoisie(1)
        End If

    End Sub

    Private Sub lbl72_Click(sender As Object, e As EventArgs) Handles lbl72.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 7 Then
            ColorieCaseChoisie(2)
        End If

    End Sub

    Private Sub lbl73_Click(sender As Object, e As EventArgs) Handles lbl73.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 7 Then
            ColorieCaseChoisie(3)
        End If

    End Sub

    Private Sub lbl74_Click(sender As Object, e As EventArgs) Handles lbl74.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 7 Then
            ColorieCaseChoisie(4)
        End If

    End Sub

    Private Sub lbl81_Click(sender As Object, e As EventArgs) Handles lbl81.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 8 Then
            ColorieCaseChoisie(1)
        End If

    End Sub

    Private Sub lbl82_Click(sender As Object, e As EventArgs) Handles lbl82.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 8 Then
            ColorieCaseChoisie(2)
        End If

    End Sub

    Private Sub lbl83_Click(sender As Object, e As EventArgs) Handles lbl83.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 8 Then
            ColorieCaseChoisie(3)
        End If

    End Sub

    Private Sub lbl84_Click(sender As Object, e As EventArgs) Handles lbl84.Click
        ' Colore la case avec la couleur sélectionnée.

        ' Vérifie le tour joué.
        If intCompteurTour = 8 Then
            ColorieCaseChoisie(4)
        End If

    End Sub

    Private Sub btnVerifier_Click(sender As Object, e As EventArgs) Handles btnVerifier.Click
        ' Déclanche la procédure de vérification.

        ValiderCoup()

    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        ' Vérifie si le joueur veut vraiment quitter la partie.

        If MessageBox.Show("Voulez-vous quitter le jeu?", "MasterMind",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Exclamation) = DialogResult.No Then
            e.Cancel = True
        End If

    End Sub

    Private Sub btnNouveau_Click(sender As Object, e As EventArgs) Handles btnNouveau.Click
        ' Redémarre et vide l'écran de jeu. Attend l'action du joueur.

        RecommencerPartie()

    End Sub

    Private Sub mnuFichierVerifier_Click(sender As Object, e As EventArgs) Handles mnuFichierVerifier.Click
        ' Déclanche la procédure de vérification.

        ValiderCoup()

    End Sub

    Private Sub mnuQuitter_Click(sender As Object, e As EventArgs) Handles mnuFichierQuitter.Click

        Me.Close()

    End Sub

    Private Sub mnuFichierNouvellePartie_Click(sender As Object, e As EventArgs) Handles mnuFichierNouvellePartie.Click
        ' Redémarre et vide l'écran de jeu. Attend l'action du joueur.

        RecommencerPartie()

    End Sub

    Private Sub frmMain_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress

        Dim intNoCouleur As Integer
        Dim strTouche As String = e.KeyChar

        ' Sélectionne la couleur avec une touche de 1 à 6.
        If strTouche > "0" AndAlso strTouche < "7" Then
            Integer.TryParse(strTouche, intNoCouleur)
            ActiveCouleur(intNoCouleur)
        End If

        ' Colorie la case avec la couleur sélectionnée entre A, B, C ou D.
        If strTouche.ToUpper = "A" Then
            ColorieCaseChoisie(1)
        ElseIf strTouche.ToUpper = "B" Then
            ColorieCaseChoisie(2)
        ElseIf strTouche.ToUpper = "C" Then
            ColorieCaseChoisie(3)
        ElseIf strTouche.ToUpper = "D" Then
            ColorieCaseChoisie(4)
        End If

    End Sub

    Private Sub mnuAidePropos_Click(sender As Object, e As EventArgs) Handles mnuAidePropos.Click
        ' Affiche une boîte de message avec le numéro de dossier de l'étudiant concepteur.

        MessageBox.Show("Numéro d'étudiant: 111245796", "À propos",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub mnuAideAide_Click(sender As Object, e As EventArgs) Handles mnuAideAide.Click
        ' Affiche une boîte de message avec une explication du fonctionnement du jeu.

        Dim strAideFormate As String = "RÈGLES DU MASTERMIND" & ControlChars.NewLine & ControlChars.NewLine &
                                       "Le but du Mastermind est de découvrir en moins de 8 essais une combinaison de 4 billes de couleurs parmi 6 couleurs possibles." & ControlChars.NewLine & ControlChars.NewLine &
                                       "A chaque essai, le joueur reçoit des indications sur les couleurs et les emplacements qu'il a choisis:" & ControlChars.NewLine & ControlChars.NewLine &
                                       "• un pion noir indique une bille bien placée." & ControlChars.NewLine &
                                       "• un pion blanc indique une bille de la bonne couleur mais mal placée." & ControlChars.NewLine & ControlChars.NewLine &
                                       "La stratégie consiste à choisir les couleurs et leur emplacement en fonction des coups précédents." & ControlChars.NewLine & ControlChars.NewLine &
                                       "Le but est d'obtenir le plus d'informations et de se rapprocher le plus rapidement possible de la solution puisque le nombre de propositions est limité."


        MessageBox.Show(strAideFormate, "Aide", MessageBoxButtons.OK)

    End Sub
End Class
