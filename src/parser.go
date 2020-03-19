package main

import (
	"flag"
	"fmt"
	"os"
	"strconv"
	"strings"

	dem "github.com/markus-wa/demoinfocs-golang"
	events "github.com/markus-wa/demoinfocs-golang/events"
)

func main() {
	pDemoFile := flag.String("demoPath", "", "path to the demo file to parse")
	//pTableType := flag.String("tableName", "", "which table to output")
	pIncludeHeaders := flag.Bool("includeHeaders", false, "include the headers for the table?")
	flag.Parse()

	f, err := os.Open(*pDemoFile)
	if err != nil {
		panic(err)
	}
	defer f.Close()

	p := dem.NewParser(f)

	csv := ""
	if *pIncludeHeaders {
		csv = "KillerName\tAssisterName\tVictimName\tWeapon\tPenetratedObjects\tHeadshot\n"
	}

	p.RegisterEventHandler(func(e events.Kill) {
		if p.GameState().IsMatchStarted() {
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
		}
	})

	err = p.ParseToEnd()
	if err != nil {
		panic(err)
	}

	fmt.Print(csv)
}
