import Config from "../../config";
import { Axios_Put } from "./Axios/Axios_Put";

export class Put_Request {

    constructor(hyperLink, body) {
        this.hyperLink = hyperLink;
        this.body = body;
    }


    put_request() {
        let client = null;
        const http_client = Config.http_client;
        switch(http_client) {
            case "axios":
                client = new Axios_Put(this.hyperLink, this.body);
                return client.put_axios_request().then(
                    (res) => {
                        return res;
                    }
                )
        }
    }

    put_request_no_body() {
        let client = null;
        const http_client = Config.http_client;
        switch(http_client) {
            case "axios":
                client = new Axios_Put(this.hyperLink);
                return client.put_axios_request().then(
                    (res) => {
                        return res;
                    }
                )
        }
    }

}