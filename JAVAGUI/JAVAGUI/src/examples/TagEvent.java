/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package examples;

import com.jacob.com.Variant;
import java.util.Observable;

/**
 *
 * @author yjbian
 */
public class TagEvent extends Observable {

    public void OnTagValueChangedEvent(Variant[] v) {
 
        this.setChanged();
        this.notifyObservers(v);

    }

}

 