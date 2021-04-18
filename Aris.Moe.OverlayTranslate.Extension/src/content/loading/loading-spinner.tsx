import * as React from "react";
import "./loading-spinner.css"
import {Observable, Subscription} from "rxjs";

export class LoadingSpinner extends React.Component<LoadingSpinnerProps, {
    loading: boolean
}> {
    subscription: Subscription;

    constructor(props: LoadingSpinnerProps) {
        super(props);
        this.state = {
            loading: true
        }

        this.subscription = props.observable.subscribe(() => {
            this.setState({
                loading: false
            })
        })
    }

    componentWillUnmount() {
        this.subscription.unsubscribe();
    }

    render() {
        let body = chrome.runtime.getURL("honyaku-chan-upper-body.png");
        let question = chrome.runtime.getURL("question.png");
        let questionQuestion = chrome.runtime.getURL("questionquestion.png");
        let questionQuestionQuestion = chrome.runtime.getURL("questionquestionquestion.png");
        let exclamation = chrome.runtime.getURL("exclamation.png");

        if (this.state.loading) {
            return (
                <div className="honyaku-loading-spinner-container">
                    <div className="honyaku-loading-spinner">
                        <img className="honyaku-question-mark honyaku-question-marks-1" src={question}/>
                        <img className="honyaku-question-mark honyaku-question-marks-2" src={questionQuestion}/>
                        <img className="honyaku-question-mark honyaku-question-marks-3" src={questionQuestionQuestion}/>
                        <img className="honyaku-body" src={body}/>
                    </div>
                </div>
            );
        } else {
            return (
                <div className="honyaku-loading-spinner-container">
                    <div className="honyaku-loading-spinner">
                        <img className="honyaku-question-mark honyaku-exclamation-mark" src={exclamation}
                             onAnimationEnd={() => this.onLoadingEnd()}/>
                        <img className="honyaku-body" src={body}/>
                    </div>
                </div>
            );
        }
    }

    private onLoadingEnd() {
        this.props.loadingAnimationDone();
    }
}

export class LoadingSpinnerProps {
    observable!: Observable<any>;
    loadingAnimationDone!: () => void;
}