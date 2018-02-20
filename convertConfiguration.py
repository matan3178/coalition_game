import sys
import math

def appe(data, dataToAppend):
	data.append(dataToAppend)
	data.append(",")


def trimGames(stream):
	stream = stream.replace(",,","")
	stream = stream.replace(",\n","\n")
	return stream


def zeroToOne(number):
	if number == 0:
		return 1
	return number


def configurationTransformation(inputFileName, outputFileName):
	# filename = sys.argv[1]
	filefd = open(inputFileName, "r")
	games = trimGames(filefd.read())
	games = games.split("*,*\n")
	newGames = "[\n"
	games = games[:-1]
	for game in games:
		game = game.split("\n")		
		curr = []
		currGame = game[0].split(",")
		num_players = int(currGame[1], 10)
		num_rounds = int(currGame[0], 10)
		appe(curr, currGame[1]) ##players
		appe(curr, currGame[0]) ##rounds
		
		for i in range(0, int(currGame[1], 10)): #players weights
			appe(curr, currGame[2+i])
		
		division = game[1:]
		for i in range(0, num_rounds): #for #round
			for j in range(0, num_players): #for #players
				appe(curr, str(int(zeroToOne(math.ceil(float(division[j].split(",")[i]))))))
		curr = "".join(curr)
		curr = "[" + curr + "30,20],\n"
		newGames = newGames + curr

	newGames = newGames[:-2]
	newGames = "NE\n" + newGames + "\n]"
	filefd = open(outputFileName, "w")
	filefd.write(newGames)
	filefd.close()

if __name__ == '__main__':
	if len(sys.argv)<3:
		print("USAGE : convertConfiguration.py <inputFileName> <outputFileName>")
		exit()
	configurationTransformation(sys.argv[1], sys.argv[2])