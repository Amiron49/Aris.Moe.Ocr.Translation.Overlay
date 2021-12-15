export class ArrayHelper {
    public static except<T>(something: T[], except: T[], subSelector: (element: T) => any): T[] {
        return something.filter(x => !except.find(y => subSelector(y) == subSelector(x)));
    }

    public static toArray<T extends Element>(something: HTMLCollectionOf<T>): T[] {
        let array: T[] = [];

        for (let i = 0; i < something.length; i++)
            array.push(something.item(i)!);

        return array;
    }
}