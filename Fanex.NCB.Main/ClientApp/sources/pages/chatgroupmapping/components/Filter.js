import React, { Component } from 'react';
import { connect } from 'react-redux';

class Filter extends Component {
    constructor(props) {
        super(props);
    }

    togglePopup() {

    }

    render() {
        return (
            <form className="filter">
                <div className="form-group">
                    <label htmlFor="chatbot" className="form-label">Bot Name</label>
                    <select name="chatbot" required="" id="chatbot" className="form-control">
                        <option>All</option>
                    </select>
                </div>
                <div className="form-group">
                    <label htmlFor="recipientType" className="form-label">Recipient Type</label>
                    <select name="recipientType" required="" id="recipientType" className="form-control">
                        <option>All</option>
                    </select>
                </div>
                <div className="form-group">
                    <label htmlFor="recipient" className="form-label">Recipient</label>
                    <input type="text" name="recipient" id="recipient" className="form-control" />
                </div>
                <div className="form-group">
                    <label htmlFor="mappingStatus" className="form-label">Mapping Status</label>
                    <select name="mappingStatus" required="" id="mappingStatus" className="form-control">
                        <option>All</option>
                    </select>
                </div>
                <button className="btn-submit">Submit</button>
            </form>
        );
    }
}

export default connect()(Filter);
