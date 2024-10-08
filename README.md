File format:

#número total de estados estado1 estado2 …

#número de estados finales estadoFinal1 estadoFinal2 …

#número total de símbolos del alfabeto simbolo1 simbolo2 … símbolo n

--TABLA DE TRANSICIONES--

TANTAS FILAS COMO ESTADOS

TANTAS COLUMNAS COMO SÍMBOLOS DEL ALFABETO + 1 (cadena vacía).

Cada columna finaliza con el símbolo # 

ej.

#6 q0 q1 q2 q3 q4 q5

#2 q4 q5

#2 0 1

--TABLA DE TRANSICIONES--

q1#q0##

q1#q2##

q3#q2##

q3#q4##

q5#q4##

q5#q2##

DO NOT USE SPACES BETWEEN '#' WHEN IS NOT NECESSARY, PROGRAM COULD CRASH.
