/**A queue that keeps its elements sorted from lowest priority to highest. */
export class PriorityQueue<T> {
  /**
   * Constructs a new PriorityQueue using the given sorting function.
   * @param sortingFunc A function that compares a given item with another.
   *
   * Return any number less than 0 if the given item has less priority than the comparison.
   * Return 0 if the items are equal in priority.
   * Return any number greater than 0 if the given item has more priority than the comparison.
   */
  public constructor(sortingFunc: (item: T, comparison: T) => number) {
    this.sortFunction = sortingFunc;
    this.heap = [];
  }

  private sortFunction: (item: T, comparison: T) => number;
  private heap: T[];

  public get length(): number {
    return this.heap.length;
  }

  /**
   * Inserts an item into the PriorityQueue.
   * @param item The item to insert into the PriorityQueue.
   */
  public insert(item: T): void {
    if (this.heap.length === 0) {
      this.heap.push(item);
      return;
    }

    // Insert item into the front of the array (back of the queue) if it is less than or equal
    // to the priority of the current lowest priority item
    if (this.sortFunction(item, this.heap[0]) <= 0) {
      this.heap.splice(0, 0, item);
      return;
    }

    // Insert item into the back of the array (front of the queue) if it is greater than or equal
    // to the priority of the current greatest priority item
    if (this.sortFunction(item, this.heap[this.heap.length - 1]) >= 0) {
      this.heap.push(item);
      return;
    }
    
    let index = Math.floor(this.heap.length / 2);
    let lowIndex = 0; // The highest checked index that has a lower priority than the item being added
    let highIndex = this.heap.length - 1; // The lowest checked index that has a higher priority than the item being added
    while (index !== 0 && index !== this.heap.length - 1) {
      let sortValue = this.sortFunction(item, this.heap[index]);

      // If both items are equal in priority, insert item here
      if (sortValue === 0) {
        this.heap.splice(index, 0, item);
        return;
      }

      // If inserted item is less priority than index
      if (sortValue <= -1) {
        let neighborValue = this.sortFunction(item, this.heap[index - 1]);

        // If inserted item is higher or equal priority than neighboring index, insert here
        if (neighborValue >= 0) {
          this.heap.splice(index, 0, item);
          return;
        }
        // Inserted item is lower priority than the neighbor who is lower priority than the index
        else {
          highIndex = index;
          index = Math.floor((index + lowIndex) / 2);
        }
      }

      // Inserted item is higher priority than index
      else {
        let neighborValue = this.sortFunction(item, this.heap[index + 1]);

        // If inserted item is lower or equal priority than neighboring index, insert here
        if (neighborValue <= 0) {
          this.heap.splice(index, 0, item);
          return;
        }
        // Inserted item is higher priority than the neighbor who is higher priority than index
        else {
          lowIndex = index;
          index = Math.floor((highIndex + index) / 2);
        }
      }
    }

    throw new Error("Could not insert item in PriorityQueue!");
  }

  /**Dequeues the highest priority item from the PriorityQueue and returns it. */
  public dequeue(): T {
    return this.heap.pop();
  }

  /**
   * Dequeues a specified amount of the highest priority items from the PriorityQueue and returns them.
   * @param amount The number of values to dequeue.
   */
  public take(amount: number): T[] {
    let values: T[] = [];
    for (var i = 0; i < amount; i++) {
      values.push(this.heap.pop());
    }
    return values;
  }
}

/**A standard dictionary that can use objects as key values. */
export class Dictionary<K, V> {
  /**
   * Constructs a new Dictionary object that stores key value pairs using the given key generator.
   * @param keyGen The function used to convert an object into a unique string used to store a KeyValuePair object.
   */
  constructor(keyGen: (item: K) => string) {
    this.dictionary = {};
    this.keyGen = keyGen;
  }
  private dictionary: Record<string, KeyValuePair<K, V>>;
  private keyGen: (item: K) => string;

  public get length(): number {
    return Object.keys(this.dictionary).length;
  }

  /**
   * Gets stored data that corresponds to the given object key.
   * @param keyObject The object used as the key to retrieve a value from the dictionary.
   */
  public getValue(keyObject: K): V {
    let key = this.keyGen(keyObject);
    if (this.dictionary[key] == null) return null;
    return this.dictionary[key].value;
  }

  /**Gets all values stored in the Dictionary. */
  public getAllValues(): V[] {
    let values: V[] = [];
    for (var key in Object.keys(this.dictionary)) {
      values.push(this.dictionary[key].value);
    }
    return values;
  }

  /**Gets all keys stored in the Dictionary. */
  public getAllKeys(): K[] {
    let keys: K[] = [];
    for (var key in Object.keys(this.dictionary)) {
      keys.push(this.dictionary[key].key);
    }
    return keys;
  }

  /**
   * Sets a new key value pair in the Dictionary.
   * @param keyObject The object used as the key for the Dictionary.
   * @param value The value stored for the corresponding key in the Dictionary.
   */
  public setValue(keyObject: K, value: V): void {
    let obj = new KeyValuePair<K, V>();
    obj.key = keyObject;
    obj.value = value;

    let key = this.keyGen(keyObject);
    this.dictionary[key] = obj;
  }

  /**
   * Removes the value corresponding to the key in the dictionary.
   *
   * Returns the KeyValuePair object that was removed.
   * @param keyObject The object used as the key to find the KeyValuePair in the Dictionary.
   */
  public remove(keyObject: K): KeyValuePair<K, V> {
    let key = this.keyGen(keyObject);
    let obj = this.dictionary[key];
    delete this.dictionary[key];

    if (obj == null) return null;
    else return obj;
  }
}

/**Represents a key value pair object used in a Dictionary.*/
export class KeyValuePair<K, V> {
  public constructor(
    fields?: {
      key?: K,
      value?: V
    }) {
    if (fields != null) {
      this.key = (fields.key != null) ? fields.key : null;
      this.value = (fields.value != null) ? fields.value : null;
    }
  }

  public key: K;
  public value: V;
}
