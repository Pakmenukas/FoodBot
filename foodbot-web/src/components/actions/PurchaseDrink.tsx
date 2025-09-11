'use client'

import kitchenAPi from "@/services/kitchenApi";
import {useEffect, useState} from "react";

export default function PurchaseDrink() {

    const [purchasedDrinks, setPurchasedDrinks] = useState<number>(0)

    const purchaseDrink = async () => {
        const result = await kitchenAPi.purchaseDrink();
        result.onSuccess((x) => setPurchasedDrinks(x.totalPurchasedCount))
    }

    useEffect(() => {
        kitchenAPi.getMyCount()
            .then((res) => res.onSuccess((x) => setPurchasedDrinks(x.totalPurchasedCount)))
    }, [])

    return (
        <div className="flex gap-2 items-center">
            <button className="btn btn-primary rounded-field" onClick={purchaseDrink}>Purchase drink</button>
            <span>Purchased: {purchasedDrinks}</span>
        </div>
    )
}