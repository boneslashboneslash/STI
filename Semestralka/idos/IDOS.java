package idos;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.PriorityQueue;
import java.util.Scanner;

public class IDOS {
    
    private static Scanner sc = new Scanner(System.in);
    
    private static PriorityQueue<Vertex> queue;
    
    // Pole vrcholu, vrchol ma hrany atd.
    private static Vertex[] graph;
    
    private static Vertex vertexFrom;
    
    private static Vertex vertexTo;
    
    private static Edge edge;

    // List dotazu na spojeni, plni se v metode output na konci 
    private static ArrayList<int[]> queries = new ArrayList<>();

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {

        // Zpravcovani vstupu
        input();

        // Zpracovani vystupu @TODO
        output();
    }

    /**
     * Zpracovani dat ze standardniho vstupu
     *
     */
    private static void input() {
        
        int numberOfLines, from, startTime, to, duration;

        // Precte pocet stanic - ty v kodu nevyuzijeme
        sc.nextInt();

        // Pocet spojeni
        numberOfLines = sc.nextInt();

        // Vytovreni prioritni fronty o vychozi delce poctu spojeni * 2
        queue = new PriorityQueue<>(numberOfLines * 2);

        // Zpracovava zadavana spojeni
        for (int i = 0; i < numberOfLines; i++) {

            // Cislo vychoziho vrcholu
            from = sc.nextInt();

            // Cislo ciloveho vrcholu
            to = sc.nextInt();

            // Cas odjezdu
            startTime = sc.nextInt();

            // Doba trvani cesty
            duration = sc.nextInt();

            // Vychozi vrchol
            vertexFrom = new Vertex(from, startTime);

            // Cilovy vrchol
            vertexTo = new Vertex(to, startTime + duration);

            // Vytvori novou hranu
            edge = new Edge(vertexTo, duration);

            // Prida hranu k vychozimu vrcholu
            vertexFrom.addEdge(edge);

            // Pridani vychoziho vrcholu do fronty, pokud tam jiz neni 
            if (!searchSame(vertexFrom, edge)) {
                queue.add(vertexFrom);
            }

            // Pridani ciloveho vrcholu do fronty, pokud tam jiz neni
            if (!searchSame(vertexTo, edge)) {
                queue.add(vertexTo);
            }
            
        }

        // Prida dalsi hrany
        otherEdges();

        // Zpracovani dotazu na spojeni
        queries();
        
        vypis();
    }
    
    /**
     * Hleda stejny vrchol ve fronte vrcholu
     *
     * @param vertex hledany vrchol
     * @param e hrana v hledaneho vrcholu
     * @return boolean true - jiz existuje, jinak false
     */
    private static boolean searchSame(Vertex vertex, Edge e) {
        Vertex v;
        graph = queue.toArray(new Vertex[queue.size()]);
        Arrays.sort(graph, queue.comparator());
        
        for (int i = 0; i < graph.length; i++) {

            // Pro stejne vrcholy se prida k nalezenemu vrcholu hrana, 
            // ktera by se jinak pridala k vkladanemu vrcholu
            if (vertex.equals(graph[i])) {
                graph[i].addEdge(e);
                return true;
            }

            // Pro vetsi cislo vrcholu jiz neni treba dal hledat 
            if (graph[i].getIdentification() > vertex.getIdentification()) {
                return false;
            }
        }
        return false;
    }


    /**
     * Pridava k vrcholum hrany, ktere vedou mezi vrcholy se stejnym
     * identifikatorem, ale jinym casem
     */
    private static void otherEdges() {
        graph = queue.toArray(new Vertex[queue.size()]);
        Arrays.sort(graph, queue.comparator());
        
        for (int i = 0; i < graph.length - 1; i++) {
            
            if (graph[i + 1].getIdentification() == graph[i].getIdentification()) {
                edge = new Edge(graph[i + 1], graph[i + 1].getTime() - graph[i].getTime());
                graph[i].addEdge(edge);
                
            }
        }
    }
    
    
    /**
     * Zpracovava zadavane dotazy na spojeni a vklada je do Listu queries
     */
    private static void queries() {
        // Pocet dotazu na spojeni
        int numberOfQueries = sc.nextInt();

        // Zpracovava zadavane dotazy na spojeni
        for (int j = 0; j < numberOfQueries; j++) {
            int[] query = new int[3];

            // Odkud
            query[0] = sc.nextInt();
            // Kam
            query[1] = sc.nextInt();
            // Od kdy
            query[2] = sc.nextInt();

            // Pridany dotazu mezi ostatni dotazy
            queries.add(query);
        }
    }

    /**
     * Vypis vysledku hledani
     */
    private static void output() {
        
    }

    /**
     * Teostovaci metoda
     */
    private static void vypis() {
        System.out.println("");
        System.out.println("Prioritní fronta");
        
        while (queue.size() > 0) {
            System.out.println(queue.poll());
        }
        
        System.out.println("Pole vrcholu - graf");
        if (graph != null) {
            //Arrays.sort(array, queue.comparator());
            for (int i = 0; i < graph.length; i++) {
                System.out.println(graph[i]);
                
            }
        }
        
        System.out.println("Dotazy");
        for (int[] q : queries) {
            System.out.println(Arrays.toString(q));
            
        }

        System.out.println("");
    }
    
}
