Imports System
Imports System.IO
Imports Microsoft.VisualBasic.FileIO

Module Main
    Sub Main()
        ' Richiedi all'utente di inserire il percorso del file CSV
        Console.WriteLine("Inserisci il percorso del file CSV:")
        Dim filePath As String = Console.ReadLine()

        ' Carica il file CSV
        Dim maxLoads As New List(Of Double())

        Try
            Using parser As New TextFieldParser(filePath)
                parser.TextFieldType = FieldType.Delimited
                parser.SetDelimiters(",")

                Dim currentWeek As String = ""

                While Not parser.EndOfData
                    Dim fields As String() = parser.ReadFields()

                    ' Verifica se la riga contiene il nome della settimana
                    If fields.Length = 1 AndAlso Not String.IsNullOrWhiteSpace(fields(0)) Then
                        currentWeek = fields(0)
                        Continue While
                    End If

                    ' Ignora le righe vuote o con valori mancanti
                    If fields.All(Function(f) String.IsNullOrWhiteSpace(f)) Then
                        Continue While
                    End If

                    ' Ignora le righe di intestazione
                    If fields(0).ToLower() = "nome" Then
                        Continue While
                    End If

                    ' Estrai i dati relativi ai massimi carichi solo se ci sono abbastanza campi
                    If fields.Length >= 4 Then
                        Dim maxLoad As Double() = {0, 0, 0} ' Inizializza un array per i massimi carichi
                        For i As Integer = 0 To Math.Min(2, fields.Length - 2)
                            Dim value As Double = 0
                            If Not String.IsNullOrWhiteSpace(fields(i + 1)) AndAlso Double.TryParse(fields(i + 1), value) Then
                                maxLoad(i) = value
                            Else
                                ' Gestisci l'errore nel caso in cui non sia possibile convertire il valore in un numero
                                Console.WriteLine($"Errore durante l'analisi del campo {i + 1} nella riga: {String.Join(",", fields)}")
                                Exit While ' Esci dal ciclo For
                            End If
                        Next
                        maxLoads.Add(maxLoad)
                    Else
                        ' Gestisci l'errore nel caso in cui ci siano meno campi di quelli previsti
                        Console.WriteLine($"Errore: numero di campi non valido nella riga: {String.Join(",", fields)}")
                    End If

                    ' Calcola la percentuale di miglioramento solo se abbiamo almeno due dati
                    If maxLoads.Count >= 2 Then
                        Dim beforeLoad As Double = maxLoads(maxLoads.Count - 2)(2) ' Prende l'ultimo elemento della settimana precedente
                        Dim afterLoad As Double = maxLoads(maxLoads.Count - 1)(2) ' Prende il primo elemento della settimana attuale

                        Dim improvementPercentage As Double = ((afterLoad - beforeLoad) / beforeLoad) * 100

                        Console.WriteLine($"Settimana: {currentWeek}, Percentuale di miglioramento: {improvementPercentage}%")
                    End If
                End While
            End Using
        Catch ex As Exception
            ' Gestisci l'errore generale
            Console.WriteLine($"Errore durante l'elaborazione del file CSV: {ex.Message}")
        End Try
    End Sub
End Module

