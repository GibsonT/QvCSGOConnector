package main

import (
	"fmt"
	"os"
	"strconv"
	"strings"

	dem "github.com/markus-wa/demoinfocs-golang"
	events "github.com/markus-wa/demoinfocs-golang/events"
)

func main() {
	f, err := os.Open("C:/Games/Steam/steamapps/common/Counter-Strike Global Offensive/bin/demoinfogo/testreplay.dem")
	if err != nil {
		panic(err)
	}
	defer f.Close()

	p := dem.NewParser(f)
	csv := "KillerName\tAssisterName\tVictimName\tWeapon\tPenetratedObjects\tHeadshot\n"

	p.RegisterEventHandler(func(e events.Kill) {
		killer := strings.ReplaceAll(e.Killer.Name, "\t", "")
		assister := ""
		victim := strings.ReplaceAll(e.Victim.Name, "\t", "")
		weapon := e.Weapon.Weapon.String()
		penetratedObjects := strconv.Itoa(e.PenetratedObjects)
		isHeadshot := strconv.FormatBool(e.IsHeadshot)

		if e.Assister != nil {
			assister = strings.ReplaceAll(e.Assister.Name, "\t", "")
		}

		csv += killer + "\t" + assister + "\t" + victim + "\t" + weapon + "\t" + penetratedObjects + "\t" + isHeadshot + "\n"
	})

	err = p.ParseToEnd()
	if err != nil {
		panic(err)
	}

	fmt.Print(csv)
}
