import React, { Component } from 'react';
import { connect } from 'react-redux';

class DataGrid extends Component {
    constructor(props) {
        super(props);
    }

    togglePopup() {

    }

    render() {
        return (
            <table className="tbl w-100">
                <thead>
                    <tr>
                        <th className="col-order">#</th>
                        <th className="col-bot-name">Bot Name</th>
                        <th className="col-group-id">Group ID</th>
                        <th className="col-group-name">Group Name</th>
                        <th className="col-recipient-type">Recipient Type</th>
                        <th className="col-recipient">Recipient</th>
                        <th className="col-status">Mapping Status</th>
                        <th>Remark</th>
                        <th className="col-action">Action</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td className="text-center">1</td>
                        <td>1on1Notify</td>
                        <td>7669657720736B79</td>
                        <td>OneOnOne & ABCChet</td>
                        <td>Licensee</td>
                        <td>12bet</td>
                        <td><span className="badge-green">Mapped</span></td>
                        <td></td>
                        <td className="text-center col-action">
                            <span className="icon-edit" onClick={this.togglePopup}></span>
                        </td>
                    </tr>
                    <tr>
                        <td className="text-center">2</td>
                        <td>1on1Notify</td>
                        <td>7669657720736B79</td>
                        <td>OneOnOne & 12bet</td>
                        <td></td>
                        <td></td>
                        <td><span className="badge-red">Not Mapped</span></td>
                        <td></td>
                        <td className="text-center col-action">
                            <span className="icon-edit"></span>
                        </td>
                    </tr>
                    <tr>
                        <td className="text-center">3</td>
                        <td>CSDNotify</td>
                        <td>7669657720736B79</td>
                        <td>ONEworks & Virtual Sports</td>
                        <td></td>
                        <td></td>
                        <td><span className="badge-red">Not Mapped</span></td>
                        <td></td>
                        <td className="text-center col-action">
                            <span className="icon-edit"></span>
                        </td>
                    </tr>
                    <tr>
                        <td className="text-center">4</td>
                        <td>OWNNotify</td>
                        <td>7669657720736B79</td>
                        <td>CSD & BBIN</td>
                        <td>3rd party product</td>
                        <td>Virtual Sport</td>
                        <td><span className="badge-green">Mapped</span></td>
                        <td></td>
                        <td className="text-center col-action">
                            <span className="icon-edit"></span>
                        </td>
                    </tr>
                    <tr>
                        <td className="text-center">4</td>
                        <td>1on1Notify</td>
                        <td>7669657720736B79</td>
                        <td>CSD & BBIN</td>
                        <td>Licensee</td>
                        <td></td>
                        <td><span className="badge-grey">Bot Removed</span></td>
                        <td></td>
                        <td className="text-center col-action">
                            <span className="icon-edit"></span>
                        </td>
                    </tr>
                    <tr>
                        <td className="text-center">4</td>
                        <td>OWNNotify</td>
                        <td>7669657720736B79</td>
                        <td>CSD & BBIN</td>
                        <td>3rd party product</td>
                        <td>Racing</td>
                        <td><span className="badge-green">Mapped</span></td>
                        <td></td>
                        <td className="text-center col-action">
                            <span className="icon-edit"></span>
                        </td>
                    </tr>
                </tbody>
            </table>
        );
    }
}

export default connect()(DataGrid);
