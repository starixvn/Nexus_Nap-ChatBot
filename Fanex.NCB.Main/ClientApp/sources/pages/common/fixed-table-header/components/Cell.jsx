import React, { Component } from 'react';
import PropTypes from 'prop-types';

class Cell extends Component {
    constructor(props) {
        super(props);
    }

    static propTypes = {
        header: PropTypes.bool,
        sortable: PropTypes.bool,
        sorting: PropTypes.bool,
        asc: PropTypes.bool,
        onClick: PropTypes.func
    }

    static defaultProps = {
        header: false,
        sortable: false,
        sorting: false,
        asc: true,
        // nothing by default
        onClick: function () { }
    }

    renderSorting(sorting, asc) {
        return <span className={`${sorting ? (asc ? 'asc' : 'desc') : ''}`} />;
    }

    renderHeader(rest) {
        const { sortable, sorting, asc, onClick } = this.props;
        return (sortable ? (<th {...rest} className={`sortable ${sorting ? 'sorting' : ''}`} onClick={onClick}>
            {this.props.children}
            {this.renderSorting(sorting, asc)}
        </th>)
            : <th {...rest}>{this.props.children}</th>);
    }

    render() {
        const { header, colWidth, sortable, sorting, asc, onClick, ...rest } = this.props;
        return header
            ? this.renderHeader(rest)
            : <td {...rest}>{this.props.children}</td>;
    }
}

export default Cell;