export class ArrayHelper {
    public static except<T>(something: T[], except: T[]): T[] {
        return something.filter(x => !except.find(y => y == x));
    }

    public static toArray<T extends Element>(something: HTMLCollectionOf<T>): T[] {
        let array: T[] = [];

        for (let i = 0; i < something.length; i++)
            array.push(something.item(i)!);

        return array;
    }
}