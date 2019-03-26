import React from 'react';
import Cell from '../Cell';
import renderer from 'react-test-renderer';

test('Cell component render correctly', () => {
    const cell = renderer.create(
        <Cell>Cell Content</Cell>
    );

    const tree = cell.toJSON();

    expect(tree).toMatchSnapshot();
});

test('Cell component render cell correctly with html attributes', () => {
    const cell = renderer.create(
        <Cell className="cell" style={{ width: 100, height: 200 }}>Cell Content</Cell>
    );

    const tree = cell.toJSON();

    expect(tree).toMatchSnapshot();
});

test('Cell component render header cell correctly with html attributes', () => {
    const cell = renderer.create(
        <Cell className="cell" style={{ width: 100, height: 200 }} header>Cell Content</Cell>
    );

    const tree = cell.toJSON();

    expect(tree).toMatchSnapshot();
});

test('Cell component render sortable header cell correctly with html attributes', () => {
    const cell = renderer.create(
        <Cell className="cell" style={{ width: 100, height: 200 }} header sortable sorting asc>Cell Content</Cell>
    );

    const tree = cell.toJSON();

    expect(tree).toMatchSnapshot();
});